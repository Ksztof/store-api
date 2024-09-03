using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Store.API.Shared.Extensions.Startup;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.JwtToken.Models;
using Store.Domain.StoreUsers;
using Store.Infrastructure.Configuration;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Services.SignalR;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add application services
builder.Services.AddApplicationServices();

// Add infrastructure services
builder.Services.AddInfrastructureServices();

// Add IUrlHelper registration
builder.Services.AddUrlHelper();

// Add API configuration (Swagger, API Explorer, etc.)
builder.Services.AddApiConfiguration();

// Add logging configuration
builder.AddLoggingConfiguration();

// Add identity configuration
builder.Services.AddIdentityServices();

// Add KeyVault integration
builder.Configuration.AddAzureKeyVaultConfiguration(builder.Configuration);

// Get key vault options
var serviceProvider = builder?.Services.BuildServiceProvider();
var keyVaultOptions = serviceProvider.GetRequiredService<IOptions<KeyVaultOptions>>().Value;

// Add database configuration with connection string from azure key vault
builder.Services.AddDatabaseConfiguration(builder.Configuration, keyVaultOptions);

// Add stripe configuration with api key from azure key vault
builder.Services.AddStripeConfiguration(keyVaultOptions);

builder.Services.AddControllers();

// Build the app
var app = builder.Build();

// Add custom middlewares
app.UseCustomMiddleware();

// Initialize database (add migration, seed initial data)
await app.InitializeDatabase();

// Add Swagger
app.UseSwaggerConfiguration(builder.Environment);

app.UseHttpsRedirection();

app.UseCustomEndpoints();

app.Run();