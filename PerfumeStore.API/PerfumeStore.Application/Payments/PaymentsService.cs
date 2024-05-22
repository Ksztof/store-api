using Microsoft.AspNetCore.Http;
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
using PerfumeStore.Domain.Entities.CarLines;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Repositories;
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


        public PaymentsService(
            PaymentIntentService paymentIntentService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<StripeOptions> stripeOptions,
            ICartsRepository cartsRepository,
            IGuestSessionService guestSessionService,
            IHttpContextService contextService)
        {
            _paymentIntentService = paymentIntentService;
            _httpContextAccessor = httpContextAccessor;
            _stripeOptions = stripeOptions.Value;
            _cartsRepository = cartsRepository;
            _guestSessionService = guestSessionService;
            _contextService = contextService;
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
                throw new StripeException($"Stripe exception has occured durring payment verification with message: {e.Message}");
            }

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var cartId = paymentIntent.Metadata["CartID"];

                // Zaktualizuj stan zamówienia w bazie danych na podstawie paymentIntent.Id
                Console.WriteLine($"PaymentIntent was successful: {paymentIntent.Id}");
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Zaktualizuj stan zamówienia w bazie danych na podstawie paymentIntent.Id
                Console.WriteLine($"PaymentIntent failed: {paymentIntent.Id}");
            }
        }
    }
}
