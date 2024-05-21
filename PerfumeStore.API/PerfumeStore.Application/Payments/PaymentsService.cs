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

        public PaymentsService(PaymentIntentService paymentIntentService)
        {
            _paymentIntentService = paymentIntentService;
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
                throw new StripeException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while processing the payment.", ex);
            }
        }
    }
}
