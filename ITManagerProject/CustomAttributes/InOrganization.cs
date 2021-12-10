using ITManagerProject.Managers;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ITManagerProject.CustomAttributes;

public class InOrganization : TypeFilterAttribute
{
    public InOrganization() : base(typeof(InOrganizationBase))
    {
        Arguments = new object[] { true };
    }

    public InOrganization(bool shouldBeIn) : base(typeof(InOrganizationBase))
    {
        Arguments = new object[] {shouldBeIn};
    }
}

internal class InOrganizationBase : ActionFilterAttribute
{
    private readonly OrganizationManager<Organization> _organizationManager;
    private readonly UserManager<User> _userManager;
    private readonly bool _shouldBeIn;
        
        
    public InOrganizationBase(OrganizationManager<Organization> organizationManager, UserManager<User> userManager, bool shouldBeIn)
    {
        _organizationManager = organizationManager;
        _userManager = userManager;
        _shouldBeIn = shouldBeIn;
    }

    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        var user = await _userManager.GetUserAsync(context.HttpContext.User);
        var status = await _organizationManager.CheckIfInAnyOrganizationAsync(user);
        if (_shouldBeIn)
        {
            if (!status)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
        else
        {
            if (status)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}