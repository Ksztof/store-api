using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Contracts.JwtToken.Models;

namespace PerfumeStore.Infrastructure.Configuration
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection(nameof(JwtOptions))
                .Bind(options);
        }
    }
}
