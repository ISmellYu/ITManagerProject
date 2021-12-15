using System;
using System.Threading.Tasks;
using ITManagerProject.CustomAttributes;
using ITManagerProject.HelperTypes;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace ITManagerProject.Controllers;

[Authorize]
[InOrganization]
public class MeetingController : Controller
{
    private readonly EventManager _eventManager;
    private readonly OrganizationManager<Organization> _organizationManager;

    public MeetingController(EventManager eventManager, OrganizationManager<Organization> organizationManager)
    {
        _organizationManager = organizationManager;
        _eventManager = eventManager;
    }
    

    public async Task<IActionResult> Index()
    {
        await HttpContext.RefreshLoginAsync();
        return View();
    }
    
    [Authorize(Policy = PolicyTypes.Organization.ManageMeetings)]
    public async Task<IActionResult> AddMeeting()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PolicyTypes.Organization.ManageMeetings)]
    public async Task<IActionResult> AddMeeting(AddMeetingViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationAsync(user.Id);
            if (viewModel.Start > viewModel.End)
            {
                ModelState.AddModelError("Start", "Data poczatkowa powinna byc mniejsza od daty końcowaej");
                return View(viewModel);
            }
            var meeting = new Event()
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                StartDate = viewModel.Start,
                EndDate = viewModel.End,
                Color = viewModel.Color,
                IsFullDay = viewModel.IsFullDay,
                Location = viewModel.Location
            };

            await _eventManager.AddEvent(meeting, organization.Id);
            return RedirectToAction("Index");

        }
        return View(viewModel);
    }

    [HttpGet]
    [InOrganization]
    [Authorize(Policy = PolicyTypes.Organization.ManageMeetings)]
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