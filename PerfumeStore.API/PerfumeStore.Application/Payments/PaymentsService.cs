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
            int cartId = int.MinValue;

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return Result.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
                cartId = await _cartsRepository.GetCartIdByUserIdAsync(userId);
            }

            if (GuestCartId != null)
                cartId = GuestCartId.Value;

            try
            {
                if (cartId != int.MinValue)
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = form.Amount,
                        Currency = form.Currency,
                        PaymentMethod = form.PaymentMethodId,
                        ConfirmationMethod = "manual",
                        Confirm = true,
                        Metadata = new Dictionary<string, string>
                        {
                            { "CartID", cartId.ToString() }
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
                    Error error = new Error("Wrong cart Id", "Cart Id cannot be 0");

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
                var exceptionError = new Error("StripeException", e.Message);
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", exceptionError);
                return;
            }

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            string cartIdMetadata = paymentIntent.Metadata["CartID"];
            int cartId;
            if (!int.TryParse(cartIdMetadata, out cartId))
            {
                Error error = new Error("StringToInt.ParsingError", "There was a problem with parsing string cart Id from metadata to int");
                await _notificationService.SendPaymentStatusAsync("unknown", "failed", error);
                return;
            }

            Order? order = await _ordersRepository.GetByCartIdAsync(cartId);
            if (order == null)
            {
                Error error = EntityErrors<Order, int>.MissingEntityByCartId(cartId);
                await _notificationService.SendPaymentStatusAsync(cartId.ToString(), "failed", error);
                return;
            }

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                order.Status = OrderStatuses.Paid;
                await _ordersRepository.UpdateAsync(order);
                await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "succeeded", null);
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                Error error = new Error("PaymentFailed", $"Payment for Order with Id: {order.Id} failed with status: PaymentIntentPaymentFailed");
                await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", error);
            }

            var scenarioExceptionError = new Error("UnexpectedScenario", "Unexpected scenario for payment verification");
            await _notificationService.SendPaymentStatusAsync(order.Id.ToString(), "failed", scenarioExceptionError);
        }
    }
}
