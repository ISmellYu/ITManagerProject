using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.HelperTypes;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PasswordGenerator;
using RandomUserSharp;

namespace ITManagerProject.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserAppContext _dbContext;
        private readonly OrganizationManager<Organization> _organizationManager;
        private readonly ILogger<DashboardController> _logger;
        private Organization _currentOrganization;

        public DashboardController(UserAppContext dbContext, OrganizationManager<Organization> organizationManager, ILogger<DashboardController> logger)
        {
            _organizationManager = organizationManager;
            _logger = logger;
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Index()
        {
            var v = new AddUserModel();
            var allRoles = _organizationManager.RoleManager.Roles.ToList();
            v.RolesList = allRoles.Select(p => new SelectListItem()
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            var allUsers = _organizationManager.UserManager.Users.ToList();
            var users = new List<User>();
            foreach (var user in allUsers)
            {
                var exists = await _organizationManager.CheckIfInAnyOrganizationAsync(user);
                if (!exists)
                    users.Add(user);
            }
            v.UsersList = users.Select(p => new SelectListItem()
            {
                Value = p.Id.ToString(),
                Text = p.UserName
            });
            return View(v);
        }

        public async Task<IActionResult> CreateOrganization()
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(user);
            if (alreadyIn)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrganization(OrganizationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var exists = await _organizationManager.CheckIfOrganizationExistsAsync(model.Name);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Dana organizacja juz istnieje!");
                    return View();
                }
                var user = await _organizationManager.UserManager.GetUserAsync(User);
                var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(user);

                if (alreadyIn)
                {
                    ModelState.AddModelError(string.Empty, "Nie mozesz stworzyc organizacji!");
                    return View();
                }
                await _organizationManager.CreateAsync(model.Name);
                
                await _organizationManager.AddToOrganizationAsync(user, model.Name);

                return RedirectToAction("Index");
            }
            
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PolicyTypes.Users.Manage)]
        public async Task<IActionResult> AddToOrganization(AddUserModel userModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                
                var currUser = await _organizationManager.UserManager.GetUserAsync(User);
                var modUser = await _organizationManager.UserManager.FindByIdAsync(userModel.UserId);
                
                var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(modUser);

                var c = await _organizationManager.UserManager.GetClaimsAsync(currUser);
                if (alreadyIn)
                {
                    ModelState.AddModelError(string.Empty, "Nie mozna dodac!");
                    return View("Index");
                }

                var rs = await _organizationManager.GetRoleForUser(currUser);
                var roleObject = await _organizationManager.RoleManager.FindByNameAsync(rs);
                var claimsBefore = await _organizationManager.RoleManager.GetClaimsAsync(roleObject) as List<Claim>;
                var role = await _organizationManager.RoleManager.FindByIdAsync(userModel.RoleId);
                await _organizationManager.AddToOrganizationAsync(modUser,
                    await _organizationManager.GetOrganizationFromUserAsync(currUser), role.Name);

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        
        
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Test()
        {
            await _organizationManager.RoleManager.SeedClaimsForRole("CEO", new List<string>()
            {
                Permissions.Users.Add,
                Permissions.Users.Edit,
                Permissions.Users.View,
                Permissions.Users.Remove,

            });
            return View();
        }
    }
}