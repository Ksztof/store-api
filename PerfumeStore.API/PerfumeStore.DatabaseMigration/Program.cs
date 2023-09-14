// See https://aka.ms/new-console-template for more information
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PerfumeShop.Serv;
using PerfumeShop.Serv.Data;
using PerfumeStore.Domain;

string connectionString = "Server=.\\SQLEXPRESS;Database=PerfumeStore;Integrated Security=True;TrustServerCertificate=true;";

IHostBuilder builder = Host.CreateDefaultBuilder(args);

var serviceProviderBuilder = new ServiceCollection();

builder.ConfigureServices(services =>
{
    services
        .AddDbContext<AspNetIdentityDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

    services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AspNetIdentityDbContext>();

    services.AddIdentityServer()
        .AddAspNetIdentity<IdentityUser>()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = b =>
                b.UseSqlServer(
                    connectionString,
                    o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b =>
                b.UseSqlServer(
                    connectionString,
                    o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
        })
        .AddDeveloperSigningCredential();

    services.AddDbContext<ShopDbContext>(options =>
        options.UseSqlServer(
            connectionString,
            o => o.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));
});

var app = builder.Build();

using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    await MigrateAsync<AspNetIdentityDbContext>(scope.ServiceProvider);

    await MigrateAsync<ConfigurationDbContext>(scope.ServiceProvider);

    await MigrateAsync<PersistedGrantDbContext>(scope.ServiceProvider);

    await MigrateAsync<ShopDbContext>(scope.ServiceProvider);

    SeedData.EnsureSeedData(scope.ServiceProvider);
}

async Task MigrateAsync<T>(IServiceProvider serviceProvider) where T : DbContext
{
    T context = serviceProvider.GetRequiredService<T>();

    await context.Database.MigrateAsync();
}