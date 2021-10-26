using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Models
{
    public class UserAppContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private DbSet<Organization<int>> Organizations { get; set; }
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