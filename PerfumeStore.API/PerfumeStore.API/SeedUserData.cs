using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.API
{

  public class SeedUserData
  {
    public static void EnsureUsers(IServiceProvider serviceProvider)
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StoreUser>>();

        var alice = userManager.FindByNameAsync("alice").Result;
        if (alice == null)
        {
          alice = new StoreUser
          {
            UserName = "alice",
            Email = "AliceSmith@email.com",
            EmailConfirmed = true,
          };
          var result = userManager.CreateAsync(alice, "Pass123$").Result;
          if (!result.Succeeded)
          {
            throw new Exception(result.Errors.First().Description);
          }
        }

        var bob = userManager.FindByNameAsync("bob").Result;
        if (bob == null)
        {
          bob = new StoreUser
          {
            UserName = "bob",
            Email = "BobSmith@email.com",
            EmailConfirmed = true,
          };
          var result = userManager.CreateAsync(bob, "Pass123$").Result;
          if (!result.Succeeded)
          {
            throw new Exception(result.Errors.First().Description);
          }
        }
      }
    }
  }
}