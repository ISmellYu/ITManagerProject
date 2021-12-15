using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.CustomAttributes;
using ITManagerProject.HelperTypes;
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
    [Authorize(Policy = PolicyTypes.Organization.ManageProjects)]
    public async Task<IActionResult> EditProject(string id)
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
        var project = await _projectManager.GetProject(Convert.ToInt32(id));
        if (project == null)
        {
            return RedirectToAction("Index");
        }

        var projectOrg = await _projectManager.GetOrganizationFromProject(project.Id);
        if (projectOrg.Id != organization.Id)
        {
            return RedirectToAction("Index");
        }

        var statuses = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Nowy", Value = "Nowy", Selected = project.Status == "Nowy" },
            new SelectListItem() { Text = "W trakcie", Value = "W trakcie", Selected = project.Status == "W trakcie" },
            new SelectListItem() { Text = "Zakonczony", Value = "Zakonczony", Selected = project.Status == "Zakonczony" }
        };

        var priorities = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Niski", Value = "Niski", Selected = project.Priority == "Niski" },
            new SelectListItem() { Text = "Normalny", Value = "Normalny", Selected = project.Priority == "Normalny" },
            new SelectListItem() { Text = "Wysoki", Value = "Wysoki", Selected = project.Priority == "Wysoki" }
        };
        var projectViewModel = new EditProjectViewModel()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Priority = project.Priority,
            Status = project.Status,
            Priorities = priorities,
            Statuses = statuses
        };
        return View(projectViewModel);
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    [Authorize(Policy = PolicyTypes.Organization.ManageProjects)]
    public async Task<IActionResult> EditProject(EditProjectViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var project = await _projectManager.GetProject(viewModel.Id);
            if (project == null)
            {
                return RedirectToAction("Index");
            }
            
            var projectOrg = await _projectManager.GetOrganizationFromProject(project.Id);
            if (projectOrg.Id != organization.Id)
            {
                return RedirectToAction("Index");
            }
            
            project.Name = viewModel.Name;
            project.Description = viewModel.Description;
            project.Priority = viewModel.Priority;
            project.Status = viewModel.Status;
            await _projectManager.UpdateProject(project);
            return RedirectToAction("Index");
            
        }

        return View(viewModel);
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