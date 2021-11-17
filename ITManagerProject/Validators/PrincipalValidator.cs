using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITManagerProject.HelperTypes;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITManagerProject.Validators
{
    public static class PrincipalValidator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            
            var manager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
            var userId = context.Principal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                context.RejectPrincipal();
                return;
            }
            
            // Get an instance using DI
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                context.RejectPrincipal();
                return;
            }
            
        }
    }
}