using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Store.API.Shared.Mapper;
using Store.API.Validation;
using Store.Application.Carts;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Email;
using Store.Application.Contracts.Guest;
using Store.Application.Contracts.JwtToken;
using Store.Application.Contracts.JwtToken.Models;
using Store.Application.Orders;
using Store.Application.Payments;
using Store.Application.Payments.SignalR;
using Store.Application.Products;
using Store.Application.Shared.Mapper;
using Store.Application.Users;
using Store.Domain.CarLines;
using Store.Domain.Carts;
using Store.Domain.Orders;
using Store.Domain.ProductCategories;
using Store.Domain.Products;
using Store.Domain.StoreUsers;
using Store.Infrastructure.Configuration;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Persistence.Repositories;
using Store.Infrastructure.Services.ContextHttp;
using Store.Infrastructure.Services.Cookies;
using Store.Infrastructure.Services.Email;
using Store.Infrastructure.Services.Guest;
using Store.Infrastructure.Services.SignalR;
using Store.Infrastructure.Services.Tokens;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Store.API.Shared.Extensions.Startup;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Application services registration
        services.AddScoped<IPaymentsService, PaymentsService>();
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<ICartsService, CartsService>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IUserService, UserService>();

        //repositories registration
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<ICartsRepository, CartsRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IProductCategoriesRepository, ProductCategoriesRepository>();
        services.AddScoped<ICartLinesRepository, CartLinesRepository>();

        //Infra services registration
        services.AddTransient<INotificationService, NotificationService>();
        services.AddScoped<IHttpContextService, HttpContextService>();
        services.AddScoped<IGuestSessionService, GuestSessionService>();
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddTransient<ICookieService, CookieService>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IPermissionService, PermissionService>();

        //validators
        services.AddTransient<IValidationService, ValidationService>();
        services.AddValidatorsFromAssemblyContaining<Program>();

        //mappers
        services.AddAutoMapper(typeof(MappingProfileApplication), typeof(MappingProfileApi));

        //configuration and handlers registration
        services.AddSingleton<JwtSecurityTokenHandler>();
        services.AddTransient<DataSeeder>();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<KeyVaultOptionsSetup>();

        return services;
    }

    internal static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<PaymentIntentService>();
        services.AddSignalR();


        return services;
    }

    internal static IServiceCollection AddUrlHelper(this IServiceCollection services)
    {
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        });

        return services;
    }

    internal static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpContextAccessor();

        return services;
    }

    internal static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        return builder;
    }

    internal static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<StoreUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ShopDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration, KeyVaultOptions keyVaultOptions)
    {
        if (string.IsNullOrEmpty(keyVaultOptions.ConnectionString))
        {
            throw new InvalidOperationException("ConnectionString is empty after configuring KeyVaultOptions.");
        }

        string? assembly = typeof(ShopDbContext).Assembly.GetName().Name;

        services.AddDbContext<ShopDbContext>(options =>
            options.UseSqlServer(keyVaultOptions.ConnectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(assembly);
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));

        return services;
    }

    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, KeyVaultOptions keyVaultOptions)
    {
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        var key = Encoding.ASCII.GetBytes(keyVaultOptions.SecurityKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtOptions.ValidIssuer,
                ValidAudience = jwtOptions.ValidAudience,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.HttpContext.Items.ContainsKey("NewAuthToken"))
                    {
                        context.Token = context.HttpContext.Items["NewAuthToken"].ToString();
                    }

                    else if (context.Request.Cookies.ContainsKey("AuthCookie"))
                    {
                        context.Token = context.Request.Cookies["AuthCookie"];
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("Visitor", policy => policy.RequireRole("Visitor"));
        });

        return services;
    }

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "MyAllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins(
                        "https://localhost:3000",
                        "https://localhost:5445",
                        "http://localhost:5002",
                        "https://dashboard.stripe.com",
                        "https://hooks.stripe.com",
                        "https://zealous-dune-0258ef303.5.azurestaticapps.net")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }

    internal static IConfigurationBuilder AddAzureKeyVaultConfiguration(this IConfigurationBuilder configuration, IConfiguration config)
    {
        var keyVaultUri = config["KeyVaultOptions:Uri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }

        return configuration;
    }

    internal static IServiceCollection AddStripeConfiguration(this IServiceCollection services, KeyVaultOptions keyVaultOptions)
    {
        StripeConfiguration.ApiKey = keyVaultOptions.StripeSecretKey;

        return services;
    }
}
