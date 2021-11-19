using System;
using ITManagerProject.Models;
using ITManagerProject.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITManagerProject.Contexts
{
    public class RefactoredIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TOrganization, TUserOrganization> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TOrganization : IdentityOrganization<TKey>
        where TUserOrganization : IdentityUserOrganization<TKey>
    {
        public RefactoredIdentityDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected RefactoredIdentityDbContext() { }
        
        public virtual string SchemaName { get; set; }

        public virtual DbSet<TOrganization> Organizations { get; set; }
        public virtual DbSet<TUserOrganization> UserOrganizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("itmanagerbeta");
            
            builder.Entity<TUser>(b =>
            {
                b.HasMany<TUserOrganization>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
                b.HasMany<OfferApplications>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
            });
            
            builder.Entity<TOrganization>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedName).HasDatabaseName("OrganizationNameIndex").IsUnique();
                b.ToTable("Organizations");

                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<TUserOrganization>().WithOne().HasForeignKey(u => u.OrganizationId).IsRequired();
                b.HasMany<OrganizationOffers>().WithOne().HasForeignKey(u => u.OrganizationId).IsRequired();
                
            });
            builder.Entity<TUserOrganization>(b =>
            {
                b.HasKey(u => new { u.UserId, u.OrganizationId });
                b.ToTable("UserOrganizations");
            });

            builder.Entity<Offer>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasMany<OrganizationOffers>().WithOne().HasForeignKey(u => u.OfferId).IsRequired();
                b.HasMany<OfferApplications>().WithOne().HasForeignKey(u => u.OfferId).IsRequired();
                b.ToTable("Offers");
            });

            builder.Entity<OrganizationOffers>(b =>
            {
                b.HasKey(u => new { u.OfferId, u.OrganizationId });
                b.ToTable("OrganizationOffers");
            });

            builder.Entity<Application>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasMany<OfferApplications>().WithOne().HasForeignKey(u => u.ApplicationId).IsRequired();
                b.ToTable("Applications");
            });

            builder.Entity<OfferApplications>(b =>
            {
                b.HasKey(u => new { u.ApplicationId, u.OfferId, u.UserId });
                b.ToTable("OfferApplications");
            });
        }
    }
}