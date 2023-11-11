using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfumeStore.API;
using PerfumeStore.Core.Configuration;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.Services;
using PerfumeStore.Core.Validators;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;
using ITokenService = PerfumeStore.Core.Services.ITokenService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Konfiguracja IdentityServerSettings
builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection("IdentityServerSettings"));

// Add services to the container.
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();
builder.Services.AddTransient<IGuestSessionService, GuestSessionService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<QuantityValidator>();
builder.Services.AddTransient<EntityIntIdValidator>();
builder.Services.AddTransient<EntityIntIdValidator>();
builder.Services.AddTransient<CreateProductFormValidator>();
builder.Services.AddTransient<UpdateProductFormValidator>();
builder.Services.AddTransient<IValidationService, ValidationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<IUrlHelper>(x =>
{
  var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
  var factory = x.GetRequiredService<IUrlHelperFactory>();
  return factory.GetUrlHelper(actionContext);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator<StoreUser>>()
  .AddTransient<IProfileService, ProfileService<StoreUser>>();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShopDbContext>(options =>
  options.UseSqlServer(connectionString, b => b.MigrationsAssembly("PerfumeStore.Domain")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString, b => b.MigrationsAssembly("PerfumeStore.Domain")));


builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. ",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  //Swagger place for token 
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "Bearer",
        Name = "Bearer",
        In = ParameterLocation.Header
      },
      new List<string>()
    }
  });
});

builder.Services.AddIdentity<StoreUser, IdentityRole>(options =>
  {
    options.SignIn.RequireConfirmedEmail = true;
  })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();
  
var identityServerSettings = builder.Configuration.GetSection("IdentityServerSettings");
var jwtSettings = builder.Configuration.GetSection("JWTSettings");

builder.Services.AddAuthentication(options =>
  {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
    options.Authority = identityServerSettings["DiscoveryUrl"];
    options.Audience = jwtSettings["validAudience"];
    options.RequireHttpsMetadata = identityServerSettings.GetValue<bool>("UseHttps");
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["JWTSettings:validIssuer"],
      ValidAudience = jwtSettings["validAudience"]
    };
  });

builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("PerfumeStore.read", policy =>
  {
    policy.RequireAuthenticatedUser();
    policy.RequireClaim("scope", "PerfumeStore.read");
  });
});

var app = builder.Build();
SeedUserData.EnsureUsers(app.Services);

using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
  ShopDbContext shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
  await shopDbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
  });
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();