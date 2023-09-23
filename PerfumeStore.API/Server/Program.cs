using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfumeShop.Serv;
using PerfumeShop.Serv.Data;

var builder = WebApplication.CreateBuilder(args);

var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
var MyLoggerFactory = LoggerFactory.Create(builder =>
{
  builder.AddConsole();
});

builder.Services.AddDbContext<AspNetIdentityDbContext>(options =>
options.UseSqlServer(defaultConnString)
  .UseLoggerFactory(MyLoggerFactory)
);


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(Config.ApiScopes)//only development
    .AddInMemoryClients(Config.Clients)//only development
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
      options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString);
    })
    .AddOperationalStore(options =>
    {
      options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString);
    })
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
});

app.Run();
