namespace PerfumeStore.Application.Contracts.Stripe.Payments
{
    public class StripeOptions
    {
        public string SecretKey { get; set; } = string.Empty;

        public string WebhookSecret { get; set; } = string.Empty;
    }
}
