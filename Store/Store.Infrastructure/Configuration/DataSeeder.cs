using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Users;
using Store.Domain.StoreUsers;

namespace Store.Infrastructure.Configuration;

public class DataSeeder
{
    private readonly UserManager<StoreUser> _userManager;
    private readonly IPermissionService _permissionService;
    private readonly KeyVaultOptions _keyVaultOptions;

    public DataSeeder(
        UserManager<StoreUser> userManager,
        IPermissionService permissionService,
        IOptions<KeyVaultOptions> keyVaultOptions)
    {
        _userManager = userManager;
        _permissionService = permissionService;
        _keyVaultOptions = keyVaultOptions.Value;
    }

    public async Task SeedDataAsync()
    {
        StoreUser? admin = await _userManager.FindByEmailAsync("kontoktoregoniktniema@gmail.com");
        if (admin == null)
        {
            StoreUser adminUser = new StoreUser
            {
                UserName = "Admin",
                Email = _keyVaultOptions.AdminMail,
                EmailConfirmed = true,
            };

            string adminPswd = _keyVaultOptions.AdminPswd;

            var result = await _userManager.CreateAsync(adminUser, adminPswd);

            await _permissionService.AssignAdminRoleAsync(adminUser);
        }
    }
}
