using System;
using Microsoft.AspNetCore.Identity;

namespace ITManagerProject.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
    }
}