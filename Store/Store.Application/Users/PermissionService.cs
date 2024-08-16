using Microsoft.AspNetCore.Identity;
using Store.Domain.Shared;
using Store.Domain.StoreUsers;

namespace Store.Application.Users
{
    public class PermissionService : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<StoreUser> _userManager;

        public PermissionService(RoleManager<IdentityRole> roleManager, UserManager<StoreUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task AssignVisitorRoleAsync(StoreUser storeUser)
        {
            string visitorRole = Roles.Visitor;

            if (!await _roleManager.RoleExistsAsync(visitorRole))
                await _roleManager.CreateAsync(new IdentityRole(visitorRole));

            if (await _roleManager.RoleExistsAsync(visitorRole))
                await _userManager.AddToRoleAsync(storeUser, visitorRole);
        }

        public async Task AssignAdminRoleAsync(StoreUser storeUser)
        {
            string adminRole = Roles.Administrator;

            if (!await _roleManager.RoleExistsAsync(adminRole))
                await _roleManager.CreateAsync(new IdentityRole(adminRole));

            if (await _roleManager.RoleExistsAsync(adminRole))
                await _userManager.AddToRoleAsync(storeUser, adminRole);
        }
    }
}