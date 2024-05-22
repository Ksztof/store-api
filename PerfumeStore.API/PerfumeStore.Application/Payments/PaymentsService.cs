using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Contracts.Stripe.Payments;
using PerfumeStore.Application.Shared.DTO.Request;
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

        public PaymentsService(
            PaymentIntentService paymentIntentService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<StripeOptions> stripeOptions)
        {
            _paymentIntentService = paymentIntentService;
            _httpContextAccessor = httpContextAccessor;
            _stripeOptions = stripeOptions.Value;
        }

        public async Task PayWithCardAsync(PayWithCardDtoApp form)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = form.Amount,
                    Currency = form.Currency,
                    PaymentMethod = form.PaymentMethodId,
                    ConfirmationMethod = "manual",
                    Confirm = true,
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
