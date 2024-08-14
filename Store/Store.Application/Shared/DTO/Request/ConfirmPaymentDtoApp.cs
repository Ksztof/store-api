namespace Store.Application.Shared.DTO.Request
{
    public class ConfirmPaymentDtoApp
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
