using System;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITManagerProject.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserAppContext _dbContext;
        private readonly OrganizationManager<Organization> _organizationManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserAppContext dbContext, OrganizationManager<Organization> organizationManager, ILogger<DashboardController> logger)
        {
            _organizationManager = organizationManager;
            _logger = logger;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateOrganization()
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(user);
            if (alreadyIn)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrganization(OrganizationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var exists = await _organizationManager.CheckIfOrganizationExistsAsync(model.Name);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Dana organizacja juz istnieje!");
                    return View();
                }
                var user = await _organizationManager.UserManager.GetUserAsync(User);
                var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(user);

                if (alreadyIn)
                {
                    ModelState.AddModelError(string.Empty, "Nie mozesz stworzyc organizacji!");
                    return View();
                }
                await _organizationManager.CreateAsync(model.Name);
                
                await _organizationManager.AddToOrganizationAsync(user, model.Name);

                return RedirectToAction("Index");
            }
            
            return View();
        }
        
        
    }
}