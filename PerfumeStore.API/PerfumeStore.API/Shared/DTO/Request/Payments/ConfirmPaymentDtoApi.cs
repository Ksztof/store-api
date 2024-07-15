using Stripe;

namespace PerfumeStore.API.Shared.DTO.Request.Payments
{
    public class ConfirmPaymentDtoApi
    {
        public PaymentIntent PaymentIntent { get; set; }
    }
}
