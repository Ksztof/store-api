using Microsoft.AspNetCore.Identity;
using Store.Application.Users;
using Store.Domain.StoreUsers;

namespace Store.Infrastructure.Configuration
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

                string adminPswd = "Haslo1234!"; //TODO: mvoe pswd to az secrets!!!!!!!!!!!!!!

                var result = await _userManager.CreateAsync(adminUser, adminPswd);

                await _permissionService.AssignAdminRoleAsync(adminUser);
            }
        }
    }
}
