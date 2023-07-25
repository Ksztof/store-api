using Microsoft.EntityFrameworkCore;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain;
using PerfumeStore.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();
builder.Services.AddTransient<IGuestSessionService, GuestSessionService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
        name: "GetProductByIdAsync",
        pattern: "api/products/{productId}",
        defaults: new { controller = "Products", action = "GetProductByIdAsync" }// Da się to lepiej zrobić? albo t wypieprzyć?
);
app.MapControllerRoute(
        name: "GetCartByIdAsync",
        pattern: "api/carts/{cartId}",
        defaults: new { controller = "Carts", action = "GetCartByIdAsync" }// Da się to lepiej zrobić? albo t wypieprzyć?
);



app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthorization();



app.MapControllers();

app.Run();
