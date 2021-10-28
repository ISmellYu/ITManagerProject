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
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public UserAppContext(DbContextOptions<UserAppContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.HasMany<UserOrganization>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
            });
            builder.Entity<Role>().ToTable("Roles", "dbo");
            builder.Entity<UserClaim>().ToTable("UserClaims", "dbo");
            builder.Entity<UserRole>().ToTable("UserRoles", "dbo");
            builder.Entity<UserLogin>().ToTable("UserLogins", "dbo");
            builder.Entity<RoleClaim>().ToTable("RoleClaims", "dbo");
            builder.Entity<UserToken>().ToTable("UserTokens", "dbo");
            builder.Entity<Organization>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedName).HasDatabaseName("OrganizationNameIndex").IsUnique();
                b.ToTable("Organizations");

                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserOrganization>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
            });
            builder.Entity<UserOrganization>(b =>
            {
                b.HasKey(u => new { u.UserId, u.OrganizationId });
                b.ToTable("UserOrganizations");
            });
        }
    }

}