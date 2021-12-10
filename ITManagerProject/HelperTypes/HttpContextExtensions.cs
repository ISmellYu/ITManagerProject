using System.Threading.Tasks;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ITManagerProject.HelperTypes;

public static class HttpContextExtensions
{
    public static async Task RefreshLoginAsync(this HttpContext context)
    {
        if (context.User == null)
            return;

        // The example uses base class, IdentityUser, yours may be called 
        // ApplicationUser if you have added any extra fields to the model
        var userManager = context.RequestServices
            .GetRequiredService<UserManager<User>>();   // Get the user manager from DI
        var signInManager = context.RequestServices
            .GetRequiredService<SignInManager<User>>();

        var user = await userManager.GetUserAsync(context.User);

        if(signInManager.IsSignedIn(context.User))
        {
            await signInManager.RefreshSignInAsync(user);
        }
    }
        
        
}