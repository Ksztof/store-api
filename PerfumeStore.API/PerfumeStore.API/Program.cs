using PerfumeStore.Core.Middleware;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
builder.Services.AddTransient<ICartsService, CartsService>();
builder.Services.AddTransient<ICartsRepository, CartsRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.MapControllerRoute(
	name: "GetProductById",
	pattern: "api/products/{productId}",
	defaults: new { controller = "Products", action = "GetProductByIdAsync" }// Da się to lepiej zrobić? albo t wypieprzyć?
);
app.UseMiddleware<GuestSessionMiddleware>();
/*app.MapControllerRoute(
	name: "GetProductById",
	pattern: "api/products/{productId}",
	defaults: new { controller = "Carts", action = "GetProductByIdAsync" } 
);*/


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
