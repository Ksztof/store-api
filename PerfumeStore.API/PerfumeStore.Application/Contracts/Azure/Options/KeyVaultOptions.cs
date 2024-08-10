using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Contracts.Azure.Options
{
    public class KeyVaultOptions
    {
        public string SecurityKey { get; set; } = string.Empty;
        public string StripeSecretKey { get; set; } = string.Empty;
        public string StripeWebhookSecret { get; set; } = string.Empty;
        public string ConnectionString {  get; set; } = string.Empty;
    }
}
