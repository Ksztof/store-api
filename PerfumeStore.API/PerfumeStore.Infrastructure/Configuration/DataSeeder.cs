using Microsoft.AspNetCore.Identity;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Infrastructure.Configuration
{
    public class DataSeeder
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly IPermissionService _permissionService;

        public DataSeeder(
            UserManager<StoreUser> userManager,
            IPermissionService permissionService)
        {
            _userManager = userManager;
            _permissionService = permissionService;
        }

        public async Task SeedDataAsync()
        {
            StoreUser? admin = await _userManager.FindByEmailAsync("kontoktoregoniktniema@gmail.com");
            if (admin == null)
            {
                StoreUser adminUser = new StoreUser
                {
                    UserName = "Admin",
                    Email = "kontoktoregoniktniema@gmail.com",
                    EmailConfirmed = true,
                };

                string adminPswd = "Haslo1234!";

                var result = await _userManager.CreateAsync(adminUser, adminPswd);

                await _permissionService.AssignAdminRoleAsync(adminUser);
            }
        }
    }
}
