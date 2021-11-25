using ITManagerProject.Managers;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ITManagerProject.HelperTypes
{
    public class InOrganization : ActionFilterAttribute
    {
        private readonly OrganizationManager<Organization> _organizationManager;

        public InOrganization(OrganizationManager<Organization> organizationManager)
        {
            _organizationManager = organizationManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}