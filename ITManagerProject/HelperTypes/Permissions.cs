using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity;

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
            foreach (var permission in allPermissions.Where(permission => !allClaims.Any(a => a.Type == "Permission" && a.Value == permission)))
            {
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
            }
        }
        
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Edit = "Permissions.Users.Edit";
            public const string Add = "Permissions.Users.Add";
            public const string Remove = "Permissions.Users.Remove";
        }
        
        public static class Organization
        {
            public const string Remove = "Permissions.Organization.Remove";
        }
    }
}