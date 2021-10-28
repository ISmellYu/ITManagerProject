using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

            builder.Entity<User>().ToTable("Users", "dbo");
            builder.Entity<Role>().ToTable("Roles", "dbo");
            builder.Entity<UserClaim>().ToTable("UserClaims", "dbo");
            builder.Entity<UserRole>().ToTable("UserRoles", "dbo");
            builder.Entity<UserLogin>().ToTable("UserLogins", "dbo");
            builder.Entity<RoleClaim>().ToTable("RoleClaims", "dbo");
            builder.Entity<UserToken>().ToTable("UserTokens", "dbo");
            
        }
    }

}