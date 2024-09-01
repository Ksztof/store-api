namespace Store.Application.Payments.Dto.Request
{
    public class ConfirmPaymentDtoApp
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
