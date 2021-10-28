using System;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
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
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(UserAppContext dbContext, ILogger<DashboardController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View(_dbContext.Organizations.ToList());
        }
        
        public IActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(OrganizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var org = _dbContext.Organizations.FirstOrDefault(p => p.NormalizedName == model.Name.ToUpper());
                if (org == null)
                {
                    _dbContext.Organizations.Add(new Organization()
                    {
                        Name = model.Name,
                        NormalizedName = model.Name.ToUpper()
                    });
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dana organizacja juz istnieje");
                }
            }
            
            return View();
        }
    }
}