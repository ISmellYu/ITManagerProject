using System;
using System.Threading.Tasks;
using ITManagerProject.CustomAttributes;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers;

public class MeetingController : Controller
{
    private readonly EventManager _eventManager;
    private readonly OrganizationManager<Organization> _organizationManager;

    public MeetingController(EventManager eventManager, OrganizationManager<Organization> organizationManager)
    {
        _organizationManager = organizationManager;
        _eventManager = eventManager;
    }
    

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult AddMeeting()
    {
        return View();
    }
    
    [HttpGet]
    [InOrganization]
    public async Task<IActionResult> RemoveMeeting(string id)
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationAsync(user.Id);
        var meeting = await _eventManager.DeleteEvent(Convert.ToInt32(id));
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [InOrganization]
    public async Task<JsonResult> GetEvents()
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationAsync(user.Id);
        var events = await _eventManager.GetAllEvents(organization.Id);
        return Json(events);
    }

}