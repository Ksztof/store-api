using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Result;
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
using Stripe.Forwarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Result> PayWithCardAsync(PayWithCardDtoApp form)
        {
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();
            int orderId = 0;

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return Result.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
                orderId = await _ordersRepository.GetNewestOrderIdByUserIdAsync(userId);
            }

            if (GuestCartId != null)
                orderId = await _ordersRepository.GetOrderIdByCartIdAsync(GuestCartId.Value);

            try
            {
                if (orderId > 0)
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = form.Amount,
                        Currency = form.Currency,
                        PaymentMethod = form.PaymentMethodId,
                        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                        {
                            Enabled = true
                        },
                        ReturnUrl = "https://localhost:3000/order",
                        Confirm = true,
                        Metadata = new Dictionary<string, string>
                        {
                            { "OrderId", orderId.ToString() }
                        }
                    };

                    PaymentIntent paymentIntent = await _paymentIntentService.CreateAsync(options);

                    if (paymentIntent.Status == "requires_confirmation")
                    {
                        var confirmOptions = new PaymentIntentConfirmOptions
                        {
                            PaymentMethod = form.PaymentMethodId
                        };
                        paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntent.Id, confirmOptions);
                    }

                    return Result.Success();
                }
                else
                {
                    Error error = new Error("Wrong order Id", "Cart Id cannot be 0");

                    return Result.Failure(error);
                }
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
                SignalrError exceptionError = new SignalrError("StripeException", e.Message);
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", exceptionError);
                return;
            }

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            if (paymentIntent == null)
            {
                SignalrError nullIntentError = new SignalrError("PaymentIntentNull", "PaymentIntent object is null");
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", nullIntentError);
                return;
            }

            if (!paymentIntent.Metadata.TryGetValue("OrderId", out string? orderIdMetadata))
            {
                SignalrError missingOrderIdError = new SignalrError("OrderIdMissing", "OrderId is missing in the payment intent metadata");
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", missingOrderIdError);
                return;
            }

            if (!int.TryParse(orderIdMetadata, out int orderId))
            {
                SignalrError error = new SignalrError("StripeMetadata.ParsingErrorStringToInt", "There was a problem with parsing string order Id from stripe metadata to int", ErrorType.Failure);
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", error);
                return;
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                SignalrError error = EntityErrors<Order, int>.NotFoundByOrderId(orderId);
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
                SignalrError error = new SignalrError("PaymentFailed", $"Payment for Order with Id: {order.Id} failed with status: PaymentIntentPaymentFailed");
                await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", error);
                return;
            }

            var scenarioExceptionError = new SignalrError("UnexpectedScenario", "Unexpected scenario for payment verification");
            await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", scenarioExceptionError);
        }
    }
}
