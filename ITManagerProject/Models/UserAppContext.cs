using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Models
{
    public class UserAppContext : IdentityDbContext<User, UserRole, int>
    {
        public UserAppContext(DbContextOptions<UserAppContext> options) : base(options)
        {
            
        }
    }
}