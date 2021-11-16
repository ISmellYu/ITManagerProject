﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.HelperTypes
{
    public static class Permissions
    {
        public static async Task SeedClaimsForRole(this RoleManager<Role> roleManager, string roleName, List<string> permissions)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            await roleManager.AddPermissionClaim(role, permissions);
        }
        
        public static async Task AddPermissionClaim(this RoleManager<Role> roleManager, Role role, List<string> permissions)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = permissions;
            foreach (var permission in allPermissions.Where(permission => !allClaims.Any(a => a.Type == CustomClaimTypes.Permission && a.Value == permission)))
            {
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
            }
        }

        public static async Task SeedRoles(this RoleManager<Role> roleManager, List<string> roles)
        {
            var allRolesToAdd = await roleManager.GetNotExistingRoles(roles);
            foreach (var role in allRolesToAdd)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }
        
        public static async Task<List<string>> GetNotExistingRoles(this RoleManager<Role> roleManager, List<string> roles)
        {
            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles.Where(r => !allRoles.Contains(r)).ToList();
        }

        public static class Users
        {
            public const string View = "Permission.Users.View";
            public const string Edit = "Permission.Users.Edit";
            public const string Add = "Permission.Users.Add";
            public const string Remove = "Permission.Users.Remove";
        }
        
        public static class Organization
        {
            public const string Remove = "Permission.Organization.Remove";
        }
    }
}