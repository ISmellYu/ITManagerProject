using System.Collections.Generic;
using System.Threading.Tasks;
using ITManagerProject.HelperTypes;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITManagerProject.Contexts
{
    public class UserAppContext : RefactoredIdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken, Organization, UserOrganization>
    {
        public UserAppContext(DbContextOptions<UserAppContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles").HasData(RoleTypesString.GetAllRolesToBeSeeded());
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<RoleClaim>().ToTable("RoleClaims").HasData(Permissions.GetSeedRoleClaims());
            builder.Entity<UserToken>().ToTable("UserTokens");
        }
    }

}