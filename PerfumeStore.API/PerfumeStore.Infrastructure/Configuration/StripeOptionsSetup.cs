using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Contracts.Stripe.Payments;

namespace PerfumeStore.Infrastructure.Configuration
{
    public class StripeOptionsSetup : IConfigureOptions<StripeOptions>
    {
        private readonly IConfiguration _configuration;

        public StripeOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public void Configure(StripeOptions options)
        {
            _configuration.GetSection(nameof(StripeOptions))
                            .Bind(options);
        }
    }
}
