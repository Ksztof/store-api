using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Store.Application.Contracts.Azure.Options;

namespace Store.Infrastructure.Configuration;

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
        options.SendgridKey = _configuration["SendgridKey"];
        options.AdminMail = _configuration["AdminMail"];
        options.AdminPswd = _configuration["AdminPswd"];
    }
}
