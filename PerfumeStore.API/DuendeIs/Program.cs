using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using DuendeIs;
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

  .AddTestUsers(TestUsers.Users);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

//TODO: Move to appsettingsjson now it s hardcoded
builder.Host.UseSerilog((ctx, lc) =>
{
  lc.MinimumLevel.Debug()
  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
  .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
  .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
  .MinimumLevel.Override("System", LogEventLevel.Warning)
  .WriteTo.Console(
    outputTemplate:
    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
  .Enrich.FromLogContext();
});

var app = builder.Build();

app.UseIdentityServer();

app.UseStaticFiles(); 
app.UseRouting();
app.UseAuthorization();

if (args.Contains("/seed"))
{
  Log.Information("Seeding database...");
  SeedData.EnsureSeedData(app);
  Log.Information("Done seeding Database. Exiting.");
  return;
}

app.Run();
