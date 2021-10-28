using System.Linq;
using ITManagerProject.Models;
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
        
        public IActionResult Test()
        {
            _dbContext.Add(new Organization("test")
            {
                NormalizedName = "test".ToUpper()
            });
            _dbContext.SaveChanges();
            return View();
        }
    }
}