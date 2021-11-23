using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers
{
    [Authorize]
    public class OffersController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Apply(int id)
        {
            return View(id);
        }
    }
}