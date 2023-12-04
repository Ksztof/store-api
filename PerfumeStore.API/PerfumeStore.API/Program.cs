using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PerfumeStore.API.Mapper;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.Core;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Application.Mapper;
using PerfumeStore.Application.Orders;
using PerfumeStore.Application.Products;
using PerfumeStore.Application.Users;
using PerfumeStore.Application.Validators;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Domain.Tokens;
using PerfumeStore.Infrastructure;
using PerfumeStore.Infrastructure.Emails;
using PerfumeStore.Infrastructure.Repositories;
using PerfumeStore.Infrastructure.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Add services to the container.\

builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IHttpContextService, HttpContextService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IGuestSessionService, GuestSessionService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<QuantityValidator>();
builder.Services.AddTransient<EntityIntIdValidator>();
builder.Services.AddTransient<EntityIntIdValidator>();
builder.Services.AddTransient<CreateProductFormValidator>();
builder.Services.AddTransient<UpdateProductFormValidator>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddAutoMapper(typeof(MappingProfileApplication), typeof(MappingProfileApi));
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

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

var assembly =  typeof(ShopDbContext).Assembly.GetName().Name;


builder.Services.AddDbContext<ShopDbContext>(options =>
  options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly)));

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
  .AddEntityFrameworkStores<ShopDbContext>()
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
      options.SaveToken = true;
      options.RequireHttpsMetadata = false;
      options.TokenValidationParameters = new TokenValidationParameters()
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidAudience = jwtSettings["validAudience"],
          ValidIssuer = jwtSettings["validIssuer"],
          ClockSkew = TimeSpan.Zero,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
      };
  });

builder.Services.AddControllers();

var app = builder.Build();

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