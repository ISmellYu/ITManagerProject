using System;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserAppContext _dbContext;
        public DashboardController(UserAppContext dbContext)
        {
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

            int? org = null;
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
                ModelState.AddModelError(String.Empty, "Dana organizacja juz istnieje");
            }
            
            return View();
        }
    }
}