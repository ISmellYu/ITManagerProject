using Microsoft.AspNetCore.Identity;

namespace ITManagerProject.Models;

public class Role : IdentityRole<int>
{
    public Role(string roleName) : base(roleName)
    {
            
    }

    public Role() : base()
    {
            
    }
}