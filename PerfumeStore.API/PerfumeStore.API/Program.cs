using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfumeStore.API.Shared.Mapper;
using PerfumeStore.API.Validators;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Contracts.Email;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Contracts.JwtToken.Models;
using PerfumeStore.Application.Contracts.Stripe.Payments;
using PerfumeStore.Application.Orders;
using PerfumeStore.Application.Payments;
using PerfumeStore.Application.Products;
using PerfumeStore.Application.Shared.Mapper;
using PerfumeStore.Application.SignalR;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Entities.StoreUsers;
using PerfumeStore.Domain.Repositories;
using PerfumeStore.Infrastructure.Configuration;
using PerfumeStore.Infrastructure.Persistence;
using PerfumeStore.Infrastructure.Persistence.Repositories;
using PerfumeStore.Infrastructure.Services.Cookies;
using PerfumeStore.Infrastructure.Services.Email;
using PerfumeStore.Infrastructure.Services.Guest;
using PerfumeStore.Infrastructure.Services.HttpContext;
using PerfumeStore.Infrastructure.Services.SignalR;
using Stripe;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using JwtTokenService = PerfumeStore.Infrastructure.Services.Tokens.JwtTokenService;

var builder = WebApplication.CreateBuilder(args);

/*builder.WebHost.ConfigureKestrel(options =>
{
    var kestrelConfigHttps = builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");
    var certPathHttps = kestrelConfigHttps.GetValue<string>("Path");
    var certPasswordHttps = kestrelConfigHttps.GetValue<string>("Password");
    var fullCertPathHttps = Path.Combine(Directory.GetCurrentDirectory(), certPathHttps);

    var certificateHttps = new X509Certificate2(fullCertPathHttps, certPasswordHttps);

    options.ListenLocalhost(5004, listenOptions =>
    {
        listenOptions.UseHttps(certificateHttps);
    });
    options.ListenLocalhost(5447, listenOptions =>
    {
        listenOptions.UseHttps(certificateHttps);
    });
});*/
builder.Services.AddHttpClient();

builder.Services.AddSignalR(); 
builder.Services.AddTransient<IPaymentsService, PaymentsService>();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IHttpContextService, HttpContextService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IGuestSessionService, GuestSessionService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<ICartLinesRepository, CartLinesRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfileApplication), typeof(MappingProfileApi));
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<ICookieService, CookieService>();
builder.Services.AddTransient<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});

builder.Services.AddTransient<DataSeeder>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<IValidationService, ValidationService>();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<StripeOptionsSetup>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);


builder.Services.AddSingleton<PaymentIntentService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();


var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

var assembly = typeof(ShopDbContext).Assembly.GetName().Name;

builder.Services.AddDbContext<ShopDbContext>(options =>
  options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly)));

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
  .AddEntityFrameworkStores<ShopDbContext>()
  .AddDefaultTokenProviders();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
var key = Encoding.ASCII.GetBytes(jwtOptions.SecurityKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
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
            if (context.Request.Cookies.ContainsKey("AuthCookie"))
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
                "https://hooks.stripe.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("StripeOptions"));
var stripeOptions = builder.Configuration.GetSection("StripeOptions").Get<StripeOptions>();
StripeConfiguration.ApiKey = stripeOptions.SecretKey;

var app = builder.Build();

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

// Configure the HTTP request pipeline.
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