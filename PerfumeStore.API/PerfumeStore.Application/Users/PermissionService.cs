﻿using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Users
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

        public async void AssignRoleAsync(StoreUser storeUser)
        {
            string visitorRole = Roles.Visitor;

            if (!await _roleManager.RoleExistsAsync(visitorRole))
                await _roleManager.CreateAsync(new IdentityRole(visitorRole));

            if (await _roleManager.RoleExistsAsync(visitorRole))
                await _userManager.AddToRoleAsync(storeUser, visitorRole);
        }
    }
}