using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITManagerProject.PolicyStuff
{
    [HtmlTargetElement(Attributes = "policy")]
    public class PolicyTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ClaimsPrincipal _principal;
        
        public PolicyTagHelper(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _principal = httpContextAccessor.HttpContext?.User;
        }
        
        public string Policy { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!(await _authorizationService.AuthorizeAsync(_principal, Policy)).Succeeded)
            {
                output.SuppressOutput();
            }
        }
    }
}