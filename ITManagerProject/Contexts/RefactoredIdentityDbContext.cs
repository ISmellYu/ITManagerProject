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
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<OrganizationOffer> OrganizationOffers { get; set; }
        
        public virtual DbSet<OfferApplication> OfferApplications { get; set; }
        public virtual DbSet<UserCookieRenew> UserCookieRenews { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OrganizationNotification> OrganizationNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("itmanagerbeta");
            
            builder.Entity<TUser>(b =>
            {
                b.HasMany<TUserOrganization>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
                b.HasMany<OfferApplication>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
                b.HasMany<UserCookieRenew>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
                b.HasMany<OrganizationNotification>().WithOne().HasForeignKey(u => u.AuthorId).IsRequired();
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
                b.HasMany<OrganizationOffer>().WithOne().HasForeignKey(u => u.OrganizationId).IsRequired();
                b.HasMany<OrganizationNotification>().WithOne().HasForeignKey(u => u.OrganizationId).IsRequired();

            });
            builder.Entity<TUserOrganization>(b =>
            {
                b.HasKey(u => new { u.UserId, u.OrganizationId });
                b.ToTable("UserOrganizations");
            });

            builder.Entity<Offer>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasMany<OrganizationOffer>().WithOne().HasForeignKey(u => u.OfferId).IsRequired();
                b.HasMany<OfferApplication>().WithOne().HasForeignKey(u => u.OfferId).IsRequired();
                b.ToTable("Offers");
            });

            builder.Entity<OrganizationOffer>(b =>
            {
                b.HasKey(u => new { u.OfferId, u.OrganizationId });
                b.ToTable("OrganizationOffers");
            });

            builder.Entity<Application>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasMany<OfferApplication>().WithOne().HasForeignKey(u => u.ApplicationId).IsRequired();
                b.ToTable("Applications");
            });

            builder.Entity<OfferApplication>(b =>
            {
                b.HasKey(u => new { u.ApplicationId, u.OfferId, u.UserId });
                b.ToTable("OfferApplications");
            });
            
            builder.Entity<UserCookieRenew>(b =>
            {
                b.HasKey(u => u.UserId);
                b.ToTable("UserCookieRenew");
            });

            builder.Entity<Notification>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasMany<OrganizationNotification>().WithOne().HasForeignKey(u => u.NotificationId).IsRequired();
                b.ToTable("Notifications");
            });

            builder.Entity<OrganizationNotification>(b =>
            {
                b.HasKey(u => new {u.AuthorId, u.OrganizationId, u.NotificationId});
                b.ToTable("OrganizationNotifications");
            });
            
        }
    }
}