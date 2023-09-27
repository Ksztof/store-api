using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using DuendeIs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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



var app = builder.Build();

app.UseIdentityServer();

app.MapGet("/", () => "Hello World!");

app.Run();
