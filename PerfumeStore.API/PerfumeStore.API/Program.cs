using DuendeIs.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfumeStore.Core.DTOs;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.Services;
using PerfumeStore.Core.Validators;
using PerfumeStore.Domain;
using System;
using System.Text;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ShopDbContext>(options =>
  options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

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


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

var identityServerSettings = builder.Configuration.GetSection("IdentityServerSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.Authority = identityServerSettings["DiscoveryUrl"];
    options.Audience = "PerfumeStore"; 
    options.RequireHttpsMetadata = identityServerSettings.GetValue<bool>("UseHttps");
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["JWTSettings:validIssuer"],
      ValidAudience = builder.Configuration["JWTSettings:validAudience"]
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

using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    ShopDbContext shopDbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await shopDbContext.Database.MigrateAsync();

    ApplicationDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await identityDbContext.Database.MigrateAsync();
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