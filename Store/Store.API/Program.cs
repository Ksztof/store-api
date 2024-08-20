using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Store.API.Shared.Mapper;
using Store.API.Validators;
using Store.Application.Carts;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Email;
using Store.Application.Contracts.Guest;
using Store.Application.Contracts.JwtToken;
using Store.Application.Contracts.JwtToken.Models;
using Store.Application.Orders;
using Store.Application.Payments;
using Store.Application.Products;
using Store.Application.Shared.Mapper;
using Store.Application.SignalR;
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
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JwtTokenService = Store.Infrastructure.Services.Tokens.JwtTokenService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddSignalR();

builder.Services.AddIdentity<StoreUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ShopDbContext>()
.AddDefaultTokenProviders();

// Register application services
builder.Services.AddTransient<IPaymentsService, PaymentsService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddScoped<IHttpContextService, HttpContextService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IGuestSessionService, GuestSessionService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<ICartLinesRepository, CartLinesRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfileApplication), typeof(MappingProfileApi));
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<ICookieService, CookieService>();
builder.Services.AddSingleton<JwtSecurityTokenHandler>();
builder.Services.AddTransient<DataSeeder>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<IValidationService, ValidationService>();

// Register configuration options
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<KeyVaultOptionsSetup>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddSingleton<PaymentIntentService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();

builder.Services.AddTransient<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});

// Add KeyVault integration
var keyVaultUri = builder.Configuration["KeyVaultOptions:Uri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}

var serviceProvider = builder.Services.BuildServiceProvider();
var keyVaultOptions = serviceProvider.GetRequiredService<IOptions<KeyVaultOptions>>().Value;


if (string.IsNullOrEmpty(keyVaultOptions.ConnectionString))
{
    throw new InvalidOperationException("ConnectionString is empty after configuring KeyVaultOptions.");
}

// Setup DB Context
var assembly = typeof(ShopDbContext).Assembly.GetName().Name;

builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(keyVaultOptions.ConnectionString, sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly(assembly);
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));
// Configure Stripe
StripeConfiguration.ApiKey = keyVaultOptions.StripeSecretKey;

// Authentication and Authorization
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
var key = Encoding.ASCII.GetBytes(keyVaultOptions.SecurityKey);

builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("Visitor", policy =>
        policy.RequireRole("Visitor"));
});

builder.Services.AddCors(options =>
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

builder.Services.AddControllers();

// Build the app
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JwtRefreshMiddleware>();

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedDataAsync();
}
using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    ShopDbContext shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await shopDbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseRouting();
app.UseCors("MyAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapHub<PaymentHub>("/paymentHub");

app.Run();
