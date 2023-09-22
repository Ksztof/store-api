﻿using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using PerfumeShop.Serv.Data;
using System.Security.Claims;

namespace PerfumeShop.Serv
{
  public class SeedData
  {
    public static void EnsureSeedData(IServiceProvider serviceProvider)
    {
      using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

      var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();

      EnsureSeedData(context);

      var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();

      EnsureUsers(scope);
    }

    private static void EnsureUsers(IServiceScope scope)
    {
      var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

      var angella = userMgr.FindByNameAsync("angella").Result;
      if (angella == null)
      {
        angella = new IdentityUser
        {
          UserName = "angella",
          Email = "angella.freeman@email.com",
          EmailConfirmed = true
        };
        var result = userMgr.CreateAsync(angella, "Pass123$").Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }

        result =
            userMgr.AddClaimsAsync(
                angella,
                new Claim[]
                {
                            new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                            new Claim(JwtClaimTypes.GivenName, "Angella"),
                            new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                            new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                            new Claim("location", "somewhere")
                }
            ).Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }
      }
    }

    private static void EnsureSeedData(ConfigurationDbContext context)
    {
      if (!context.Clients.Any())
      {
        foreach (var client in Config.Clients.ToList())
        {
          context.Clients.Add(client.ToEntity());
        }

        context.SaveChanges();
      }

      if (!context.IdentityResources.Any())
      {
        foreach (var resource in Config.IdentityResources.ToList())
        {
          context.IdentityResources.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }

      if (!context.ApiScopes.Any())
      {
        foreach (var resource in Config.ApiScopes.ToList())
        {
          context.ApiScopes.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }

      if (!context.ApiResources.Any())
      {
        foreach (var resource in Config.ApiResources.ToList())
        {
          context.ApiResources.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }
    }
  }
}