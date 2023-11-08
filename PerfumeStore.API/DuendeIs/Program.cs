using Duende.IdentityServer.EntityFramework.DbContexts;
using DuendeIs;
using DuendeIs.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DuendeIs.Core.Configuration;
using DuendeIs.DbContexts;
using DuendeIs.Core.Models;

Log.Information("Starting Up");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IUrlHelper>(x =>
{
  var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
  var factory = x.GetRequiredService<IUrlHelperFactory>();
  return factory.GetUrlHelper(actionContext);
});
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

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
