using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Guest;
using Store.Application.Payments.Dto.Request;
using Store.Application.Payments.SignalR;
using Store.Domain.Abstractions;
using Store.Domain.Orders;
using Store.Domain.Orders.Enums;
using Store.Domain.Shared.Errors;
using Store.Domain.StoreUsers.Errors;
using Stripe;

namespace Store.Application.Payments;

public class PaymentsService : IPaymentsService
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGuestSessionService _guestSessionService;
    private readonly IHttpContextService _contextService;
    public readonly IOrdersRepository _ordersRepository;
    private readonly INotificationService _notificationService;
    private readonly KeyVaultOptions _keyVaultOptions;

    public PaymentsService(
        PaymentIntentService paymentIntentService,
        IHttpContextAccessor httpContextAccessor,
        IGuestSessionService guestSessionService,
        IHttpContextService contextService,
        IOrdersRepository ordersRepository,
        INotificationService notificationService,
        IOptions<KeyVaultOptions> keyVaultOptions)
    {
        _paymentIntentService = paymentIntentService;
        _httpContextAccessor = httpContextAccessor;
        _guestSessionService = guestSessionService;
        _contextService = contextService;
        _ordersRepository = ordersRepository;
        _notificationService = notificationService;
        _keyVaultOptions = keyVaultOptions.Value;
    }

    public async Task<Result<string>> GetClientSecretAsync(GetClientSecretDtoApp form)
    {
        try
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return Result<string>.Failure(error);
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = form.Amount,
                Currency = form.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Confirm = false,
            };

            PaymentIntent paymentIntent = await _paymentIntentService.CreateAsync(options);
            string clientSecret = paymentIntent.ClientSecret;

            return Result<string>.Success(clientSecret);
        }
        catch (StripeException ex)
        {
            throw new StripeException($"Stripe error occurred while getting client secret with message: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred while getting client secret with message: {ex.Message}");
        }
    }


    public async Task<Result> UpdatePaymentIntentAsync(UpdatePaymentIntentDtoApp form)
    {
        try
        {
            int orderId = 0;
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return Result.Failure(error);
            }

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();

                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }

                orderId = await _ordersRepository.GetNewestOrderIdByUserIdAsync(result.Value);
            }

            if (receiveCartIdResult.IsSuccess)
            {
                orderId = await _ordersRepository.GetOrderIdByCartIdAsync(receiveCartIdResult.Value);
            }

            if (orderId <= 0)
            {
                Error error = EntityErrors<Order, int>.WrongEntityId(orderId);

                return Result.Failure(error);
            }

            string paymentIntentId = ExtractPaymentIntentIdFromClientSecret(form.clientSecret);

            if (string.IsNullOrEmpty(paymentIntentId))
            {
                throw new ArgumentException("Invalid client secret.");
            }

            PaymentIntent paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId);

            if (paymentIntent.Metadata != null && paymentIntent.Metadata.ContainsKey("OrderId"))
            {
                bool parsingResult = int.TryParse(paymentIntent.Metadata["OrderId"], out int metadataOrderId);

                if (!parsingResult)
                {
                    Error error = EntityErrors<Order, int>.ParsingEntityIdFailed();

                    return Result.Failure(error);
                }

                if (metadataOrderId != orderId)
                {
                    Error error = EntityErrors<Order, int>.EntityIdNotMatch(metadataOrderId, orderId);

                    return Result.Failure(error);
                }

                return Result.Success();
            }

            var options = new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", orderId.ToString() }
                }
            };

            paymentIntent = await _paymentIntentService.UpdateAsync(paymentIntentId, options);

            _guestSessionService.SetCartIdCookieAsExpired();

            return Result.Success();
        }
        catch (StripeException ex)
        {
            throw new StripeException($"Stripe error occurred while updating metadata with order ID with message: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred while updating metadata with order ID with message: {ex.Message}");
        }
    }


    public async Task<Result> ConfirmPaymentAsync(ConfirmPaymentDtoApp request)
    {
        try
        {
            PaymentIntent paymentIntent = await _paymentIntentService.GetAsync(request.PaymentIntentId);

            if (paymentIntent.Status == "requires_confirmation" && string.IsNullOrEmpty(request.PaymentMethodId))
            {
                throw new Exception("PaymentMethodId is required to confirm the payment.");
            }

            if (paymentIntent.Status == "requires_confirmation")
            {
                var confirmOptions = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = request.PaymentMethodId
                };

                paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntent.Id, confirmOptions);
            }

            return Result.Success();
        }
        catch (StripeException ex)
        {
            throw new StripeException($"Stripe error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred while confirming the payment: {ex.Message}");
        }
    }

    public async Task VerifyPaymentAsync()
    {
        var requestJson = await new StreamReader(_httpContextAccessor.HttpContext.Request.Body).ReadToEndAsync();

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                requestJson,
                _httpContextAccessor.HttpContext.Request.Headers["Stripe-Signature"],
                _keyVaultOptions.StripeWebhookSecret
            );
        }
        catch (StripeException e)
        {
            Error exceptionError = new Error("StripeException", e.Message);
            await _notificationService.SendPaymentStatusAsync("unknown", "failed", exceptionError);

            return;
        }

        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        if (paymentIntent == null)
        {
            Error nullIntentError = new Error("PaymentIntentNull", "PaymentIntent object is null");
            await _notificationService.SendPaymentStatusAsync("unknown", "failed", nullIntentError);

            return;
        }

        if (!paymentIntent.Metadata.TryGetValue("OrderId", out string? orderIdMetadata))
        {
            Error missingOrderIdError = new Error("OrderIdMissing", "OrderId is missing in the payment intent metadata");
            await _notificationService.SendPaymentStatusAsync("unknown", "failed", missingOrderIdError);

            return;
        }

        if (!int.TryParse(orderIdMetadata, out int orderId))
        {
            Error error = new Error("StripeMetadata.ParsingErrorStringToInt", "There was a problem with parsing string order Id from stripe metadata to int");
            await _notificationService.SendPaymentStatusAsync("unknown", "failed", error);

            return;
        }

        EntityResult<Order> getOrder = await _ordersRepository.GetByIdAsync(orderId);

        if (getOrder.IsFailure)
        {
            await _notificationService.SendPaymentStatusAsync(orderId.ToString(), "failed", getOrder.Error);

            return;
        }

        Order order = getOrder.Entity;

        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            order.Status = OrderStatuses.Paid;

            await _ordersRepository.UpdateAsync(order);

            await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "succeeded", null);

            return;
        }
        else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        {
            Error error = new Error("PaymentFailed", $"Payment for Order with Id: {order.Id} failed with status: PaymentIntentPaymentFailed");
            await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", error);

            return;
        }

        var scenarioExceptionError = new Error("UnexpectedScenario", "Unexpected scenario for payment verification");
        await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", scenarioExceptionError);
    }

    private string ExtractPaymentIntentIdFromClientSecret(string clientSecret)
    {
        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Client secret is empty.");
        }

        var parts = clientSecret.Split('_');

        if (parts.Length < 3 || !parts[0].StartsWith("pi"))
        {
            throw new InvalidOperationException("Client secret format in not valid");
        }

        return $"{parts[0]}_{parts[1]}";
    }
}