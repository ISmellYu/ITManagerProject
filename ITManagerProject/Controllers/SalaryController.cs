using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers;

public class SalaryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}