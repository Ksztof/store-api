using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PerfumeStore.Infrastructure.Emails;
using PerfumeStore.Infrastructure.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.CompositionRoot
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Rejestracja usług infrastruktury
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ITokenService, TokenService>();
            // ... inne usługi
            return services;
        }
    }
}
