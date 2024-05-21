namespace PerfumeStore.API.Shared.DTO.Request.Payments
{
    public class PayWithCardDtoApi
    {
        public string PaymentMethodId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}
