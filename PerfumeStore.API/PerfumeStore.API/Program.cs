using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Core.DTOs;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.Services;
using PerfumeStore.Core.Validators;
using PerfumeStore.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

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
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.Authority = "https://localhost:5443";
      options.Audience = "PerfumeStoreAPI";
      options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();