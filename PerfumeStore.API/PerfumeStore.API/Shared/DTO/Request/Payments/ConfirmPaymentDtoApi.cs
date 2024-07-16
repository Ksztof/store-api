using Stripe;

namespace PerfumeStore.API.Shared.DTO.Request.Payments
{
    public class ConfirmPaymentDtoApi
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
