using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.CustomAttributes;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ITManagerProject.Controllers;
[InOrganization]
[Authorize]
public class ProjectController : Controller
{
    private readonly OrganizationManager<Organization> _organizationManager;
    private readonly ProjectManager _projectManager;

    public ProjectController(OrganizationManager<Organization> organizationManager, ProjectManager projectManager)
    {
        _organizationManager = organizationManager;
        _projectManager = projectManager;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> CreateProject()
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationFromUserAsync(user);

        var users = await _organizationManager.GetAllUsersFromOrganizationAsync(organization.NormalizedName);
        var model = new ProjectViewModel();
        model.UsersList = users.Select(p => new SelectListItem()
        {
            Value = p.Id.ToString(),
            Text = p.FirstName + " " + p.LastName,
            Selected = false
        });
        return View(model);
    }
    
    
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> CreateProject(ProjectViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var orgId = (await _organizationManager.GetOrganizationFromUserAsync(user)).Id;
            var manager = await _organizationManager.UserManager.FindByIdAsync(model.ManagerId);
            await _projectManager.CreateProject(new Project()
            {
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                ManagerName = manager.FirstName + " " + manager.LastName
            }, orgId);
            return RedirectToAction("Index");
        }
        return View(model);
    }
    
    [HttpPost]
    public async Task<JsonResult> GetProjects()
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var orgId = (await _organizationManager.GetOrganizationFromUserAsync(user)).Id;
        var projects = await _projectManager.GetProjectsByOrganization(orgId);
        return Json(projects);
    }
    
    
}