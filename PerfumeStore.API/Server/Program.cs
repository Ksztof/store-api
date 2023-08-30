using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfumeShop.Serv.Data;

/*var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}*/

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

/*if (seed)
{
    SeedData.EnsureSeedData(defaultConnString);
}
*/
builder.Services.AddDbContext<AspNetIdentityDbContext>(options =>
    options.UseSqlServer(defaultConnString,
        b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    AspNetIdentityDbContext aspNetIdentityContext = scope.ServiceProvider.GetRequiredService<AspNetIdentityDbContext>();
    await aspNetIdentityContext.Database.MigrateAsync();

    ConfigurationDbContext configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    await configurationContext.Database.MigrateAsync();

    PersistedGrantDbContext persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
    await persistedGrantContext.Database.MigrateAsync();
}

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();