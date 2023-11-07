using Duende.IdentityServer.EntityFramework.DbContexts;
using DuendeIs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;
using Serilog;

Log.Information("Starting Up");
var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString, o => o.MigrationsAssembly(assembly)));

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

builder.Services.AddIdentity<StoreUser, IdentityRole>(options =>
  {
    options.SignIn.RequireConfirmedEmail = true;
  })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

var app = builder.Build();

SeedData.EnsureSeedData(app);

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();

app.Run();
