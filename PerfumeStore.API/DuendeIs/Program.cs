using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using DuendeIs;
using DuendeIs.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Information("Starting Up");
var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(assembly)));

builder.Services.AddDbContext<PersistedGrantDbContext>(options =>
  options.UseSqlServer(connectionString, o => o.MigrationsAssembly(assembly)));

builder.Services.AddDbContext<ConfigurationDbContext>(options =>
  options.UseSqlServer(connectionString, o => o.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer(options =>
{
  options.Events.RaiseErrorEvents = true;
  options.Events.RaiseInformationEvents = true;
  options.Events.RaiseFailureEvents = true;
  options.Events.RaiseSuccessEvents = true;
  options.EmitStaticAudienceClaim = true;

}).AddConfigurationStore(options =>
{
  options.ConfigureDbContext = b =>
  b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
})
  .AddOperationalStore(options =>
  {
    options.ConfigureDbContext = b =>
    b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
  })
  .AddDeveloperSigningCredential()
  .AddAspNetIdentity<IdentityUser>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

/*builder.Host.UseSerilog((ctx, lc) =>
{
  lc.ReadFrom.Configuration(ctx.Configuration);
});
*/

var app = builder.Build();

//using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
//{
//    ApplicationDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    await identityDbContext.Database.MigrateAsync();

//    PersistedGrantDbContext persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
//    await persistedGrantDbContext.Database.MigrateAsync();

//    ConfigurationDbContext configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
//    await configurationDbContext.Database.MigrateAsync();


//}

SeedData.EnsureSeedData(app);

// Uporz?dkowana kolejno?? middleware
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication(); // Dodane, wa?ne aby by?o po UseIdentityServer a przed UseAuthorization
app.UseAuthorization();

/*if (args.Contains("/seed"))
{
  Log.Information("Seeding database...");
  SeedData.EnsureSeedData(app);
  Log.Information("Done seeding Database. Exiting.");
  return;
}*/

app.Run();
