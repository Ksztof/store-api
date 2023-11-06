using Duende.IdentityServer.EntityFramework.DbContexts;
using DuendeIs;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Information("Starting Up");
var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PersistedGrantDbContext>(options =>
  options.UseSqlServer(connectionString, o => o.MigrationsAssembly(assembly)));

builder.Services.AddDbContext<ConfigurationDbContext>(options =>
  options.UseSqlServer(connectionString, o => o.MigrationsAssembly(assembly)));

builder.Services.AddIdentityServer(options =>
  {
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

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
  .AddDeveloperSigningCredential();


var app = builder.Build();

SeedData.EnsureSeedData(app);

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();

app.Run();
