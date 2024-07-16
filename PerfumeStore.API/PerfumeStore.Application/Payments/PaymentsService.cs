using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Abstractions;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Application.Contracts.Stripe.Payments;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Application.SignalR;
using PerfumeStore.Domain.Entities.CarLines;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.Orders;
using PerfumeStore.Domain.Repositories;
using PerfumeStore.Domain.Shared.Enums;
using Stripe;

namespace PerfumeStore.Application.Payments
{
    public class PaymentsService : IPaymentsService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StripeOptions _stripeOptions;
        private readonly ICartsRepository _cartsRepository;
        private readonly IGuestSessionService _guestSessionService;
        private readonly IHttpContextService _contextService;
        public readonly IOrdersRepository _ordersRepository;
        private readonly INotificationService _notificationService;

        public PaymentsService(
            PaymentIntentService paymentIntentService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<StripeOptions> stripeOptions,
            ICartsRepository cartsRepository,
            IGuestSessionService guestSessionService,
            IHttpContextService contextService,
            IOrdersRepository ordersRepository,
            INotificationService notificationService)
        {
            _paymentIntentService = paymentIntentService;
            _httpContextAccessor = httpContextAccessor;
            _stripeOptions = stripeOptions.Value;
            _cartsRepository = cartsRepository;
            _guestSessionService = guestSessionService;
            _contextService = contextService;
            _ordersRepository = ordersRepository;
            _notificationService = notificationService;
        }

        public async Task<Result<PaymentIntent>> StartOrderAsync(StartOrderDtoApp form)
        {
            try
            {
                bool isUserAuthenticated = _contextService.IsUserAuthenticated();
                int? GuestCartId = _guestSessionService.GetCartId();
                int orderId = 0;

                if (GuestCartId == null && isUserAuthenticated == false)
                {
                    Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                    return Result<PaymentIntent>.Failure(error);
                }

                if (isUserAuthenticated)
                {
                    string userId = _contextService.GetUserId();
                    orderId = await _ordersRepository.GetNewestOrderIdByUserIdAsync(userId);
                }

                if (GuestCartId != null)
                    orderId = await _ordersRepository.GetOrderIdByCartIdAsync(GuestCartId.Value);

                if (orderId <= 0)
                {
                    Error error = EntityErrors<Order, int>.WrongEntityId(orderId);

                    return Result<PaymentIntent>.Failure(error);
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = form.Amount,
                    Currency = form.Currency,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    ReturnUrl = "https://localhost:3000/order",
                    Metadata = new Dictionary<string, string>
                        {
                            { "OrderId", orderId.ToString() }
                        }
                };

                PaymentIntent paymentIntent = await _paymentIntentService.CreateAsync(options);

                return Result<PaymentIntent>.Success(paymentIntent);
            }
            catch (StripeException ex)
            {
                throw new StripeException($"Stripe error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while processing the payment: {ex.Message}");
            }
        }


        public async Task<Result> ConfirmPaymentAsync(ConfirmPaymentDtoApp request)
        {
            try
            {
                PaymentIntent paymentIntent = await _paymentIntentService.GetAsync(request.PaymentIntentId);
                if (paymentIntent.Status == "requires_confirmation")
                {
                    if (string.IsNullOrEmpty(request.PaymentMethodId))
                    {
                        throw new Exception("PaymentMethodId is required to confirm the payment.");
                    }

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
            var json = await new StreamReader(_httpContextAccessor.HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    _httpContextAccessor.HttpContext.Request.Headers["Stripe-Signature"],
                    _stripeOptions.WebhookSecret
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

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                Error error = EntityErrors<Order, int>.NotFoundByOrderId(orderId);
                await _notificationService.SendPaymentStatusAsync(orderId.ToString(), "failed", error);
                return;
            }

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
    }
}