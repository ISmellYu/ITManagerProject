using System.Collections.Generic;
using System.Threading.Tasks;
using ITManagerProject.HelperTypes;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITManagerProject.Contexts;

public class UserAppContext : RefactoredIdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken, Organization, UserOrganization>
{
    public UserAppContext(DbContextOptions<UserAppContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().ToTable("users");
        builder.Entity<Role>().ToTable("roles").HasData(RoleTypesString.GetAllRolesToBeSeeded());
        builder.Entity<UserClaim>().ToTable("userclaims");
        builder.Entity<UserRole>().ToTable("userroles");
        builder.Entity<UserLogin>().ToTable("userlogins");
        builder.Entity<RoleClaim>().ToTable("roleclaims").HasData(Permissions.GetSeedRoleClaims());
        builder.Entity<UserToken>().ToTable("usertokens");
    }
}