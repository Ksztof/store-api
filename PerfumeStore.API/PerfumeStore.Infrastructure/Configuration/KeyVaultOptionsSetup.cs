using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Contracts.Azure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Configuration
{
    public class KeyVaultOptionsSetup : IConfigureOptions<KeyVaultOptions>
    {
        private readonly IConfiguration _configuration;

        public KeyVaultOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(KeyVaultOptions options)
        {
            options.SecurityKey = _configuration["SecurityKey"];
            options.StripeSecretKey = _configuration["StripeSecretKey"];
            options.StripeWebhookSecret = _configuration["StripeWebhookSecret"];
            options.ConnectionString = _configuration["ConnectionString"];
        }
    }
}
