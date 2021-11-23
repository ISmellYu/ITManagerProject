using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers
{
    public class ProjectController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}