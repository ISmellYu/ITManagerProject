using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationManager _applicationManager;
        private readonly OfferManager _offerManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserAppContext dbContext, OrganizationManager<Organization> organizationManager, 
            RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, 
            ILogger<DashboardController> logger, ApplicationManager applicationManager, OfferManager offerManager)
        {
            _applicationManager = applicationManager;
            _offerManager = offerManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _organizationManager = organizationManager;
            _logger = logger;
            _dbContext = dbContext;
        }
        
        
        [InOrganization]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Policy = PolicyTypes.Users.View)]
        [InOrganization]
        public async Task<IActionResult> Employees()
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

                // Refreshing cookie to update claims/or smth
                await HttpContext.RefreshLoginAsync();

                return RedirectToAction("Index");
            }
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [InOrganization]
        public async Task<IActionResult> AddToOrganization(AddUserModel userModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var currUser = await _organizationManager.UserManager.GetUserAsync(User);
                var modUser = await _organizationManager.UserManager.FindByIdAsync(userModel.UserId);
                
                var alreadyIn = await _organizationManager.CheckIfInAnyOrganizationAsync(modUser);
                
                if (alreadyIn)
                {
                    ModelState.AddModelError(string.Empty, "Nie mozna dodac!");
                    return View("Index");
                }
                
                var role = await _organizationManager.RoleManager.FindByIdAsync(userModel.RoleId);
                var currOrg = await _organizationManager.GetOrganizationFromUserAsync(currUser);
                await _organizationManager.AddToOrganizationAsync(modUser, currOrg, userModel.Salary, role.Name);
                

                // Refreshing cookie to update claims/or smth
                await HttpContext.RefreshLoginAsync();
                
            }

            return RedirectToAction("Employees");
        }

        [HttpGet]
        [Authorize(Policy = PolicyTypes.Users.Edit)]
        [InOrganization]
        public async Task<IActionResult> EditEmployee(string id)
        {
            var currUser = await _organizationManager.UserManager.GetUserAsync(User);
            var modUser = await _organizationManager.UserManager.FindByIdAsync(id.ToString());

            var orgcurrUser = await _organizationManager.GetOrganizationFromUserAsync(currUser);
            var orgmodUser = await _organizationManager.GetOrganizationFromUserAsync(modUser);
            if (orgcurrUser.Id != orgmodUser.Id || currUser == modUser)
            {
                return RedirectToAction("Employees");
            }


            var model = new EditViewModel();
            model.User = modUser;
            var allRoles = _organizationManager.RoleManager.Roles.ToList();
            var rl = (await _userManager.GetRolesAsync(modUser))[0];
            model.Roles = allRoles.Select(p => new SelectListItem()
            {
                Value = p.Id.ToString(),
                Text = p.Name,
                Selected = rl == p.Name
            });
            model.Salary = modUser.Salary;
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PolicyTypes.Users.Edit)]
        [InOrganization]
        public async Task<IActionResult> EditEmployee(string id, string roleId, int salary) // Edit employee with specified id, roleid and salary
        {
            if (ModelState.IsValid)
            {
                var currUser = await _organizationManager.UserManager.GetUserAsync(User);   // Get current user
                var modUser = await _organizationManager.UserManager.FindByIdAsync(id); // Get user to edit

                if ((await _organizationManager.GetOrganizationFromUserAsync(currUser)).Id !=
                    (await _organizationManager.GetOrganizationFromUserAsync(modUser)).Id ||
                    currUser == modUser)
                {
                    return RedirectToAction("Employees");
                }

                if (roleId == null)
                {
                    return RedirectToAction("Employees");
                }

                var role = await _organizationManager.RoleManager.FindByIdAsync(roleId);
                await _organizationManager.UserManager.RemoveFromRolesAsync(modUser,
                    await _organizationManager.UserManager.GetRolesAsync(modUser));
                await _organizationManager.UserManager.AddToRoleAsync(modUser, role.Name);
                await _organizationManager.ChangeSalary(modUser, salary);   // Change salary
                

                // Refreshing cookie to update claims/or smth
                await HttpContext.RefreshLoginAsync();  // 
                
                return RedirectToAction("Employees");
            }

            return View();
        }
        
        [Authorize(Policy = PolicyTypes.Users.Edit)]
        [InOrganization]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (ModelState.IsValid)
            {
                var currUser = await _organizationManager.UserManager.GetUserAsync(User);
                var modUser = await _organizationManager.UserManager.FindByIdAsync(id.ToString());
                var org = await _organizationManager.GetOrganizationFromUserAsync(modUser);

                if ((await _organizationManager.GetOrganizationFromUserAsync(currUser)).Id != org.Id ||
                    currUser == modUser)
                {
                    return RedirectToAction("Employees");
                }

                await _organizationManager.RemoveFromOrganizationAsync(modUser, org.Name);
                // Refreshing cookie to update claims/or smth
                await HttpContext.RefreshLoginAsync();
                
                return RedirectToAction("Employees");
            }

            return RedirectToAction("EditEmployee", new {id = id.ToString()});
        }

        [Authorize(Policy = PolicyTypes.Organization.Remove)]
        [InOrganization]
        public async Task<IActionResult> RemoveOrganization()
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            await _organizationManager.RemoveAsync(organization.Name);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> AddOffer()
        {
            var f = new AddOfferViewModel();
            var allRoles = _organizationManager.RoleManager.Roles.ToList();
            f.Roles = allRoles.Select(p => new SelectListItem()
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });
            return View(f);
        }

        [HttpPost]
        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> AddOffer(AddOfferViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _organizationManager.UserManager.GetUserAsync(User);
                var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
                var role = await _organizationManager.RoleManager.FindByIdAsync(model.RoleId);
                var allOffers = await _offerManager.GetOffersByOrganization(organization);
                if (allOffers.Any(p => p.Name == model.Name && p.Description == model.Description && 
                                       p.Company == organization.Name && p.Role == role.Name && 
                                       p.Salary == model.Salary))
                {
                    ModelState.AddModelError("", "Oferta juz istnieje!");
                }

                _offerManager.AddOffer(new Offer()
                {
                    Company = organization.Name,
                    Description = model.Description,
                    Location = model.Location,
                    Name = model.Name,
                    Role = role.Name,
                    Salary = model.Salary
                }, organization.Id);
                return RedirectToAction("Offers");
            }
            
            return RedirectToAction("Offers");
        }
        
        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> RemoveOffer(int id)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var offer = await _offerManager.GetOfferById(id);
            if (offer == null)
            {
                return RedirectToAction("Offers");
            }
            if (offer.Company == organization.Name)
            {
                _offerManager.DeleteOffer(offer);
                return RedirectToAction("Offers");

            }

            return RedirectToAction("Offers");
        }

        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> AcceptApplication(string id)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var application = await _applicationManager.GetApplicationById(id);
            if (application == null)
            {
                return RedirectToAction("Applications");
            }

            var x = await _applicationManager.GetOfferByApplicationId(Convert.ToInt32(id));
            if (x == null)
            {
                return RedirectToAction("Applications");
            }

            var appUser = await _applicationManager.GetUserByApplicationId(Convert.ToInt32(id));
            
            if (x.Company == organization.Name)
            {
                await _applicationManager.AcceptApplication(new Application() { Id = Convert.ToInt32(id) }, x.Id,
                    appUser.Id);
                return RedirectToAction("Applications");
            }
            return RedirectToAction("Applications");
        }

        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> RejectApplication(string id)
        {
            var user = await _organizationManager.UserManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var application = await _applicationManager.GetApplicationById(id);
            if (application == null)
            {
                return RedirectToAction("Applications");
            }

            var x = await _applicationManager.GetOfferByApplicationId(Convert.ToInt32(id));
            if (x == null)
            {
                return RedirectToAction("Applications");
            }
            
            
            if (x.Company == organization.Name)
            {
                await _applicationManager.RemoveApplication(new Application()
                {
                    Id = Convert.ToInt32(id),
                }, x.Id);
                return RedirectToAction("Applications");
            }
            return RedirectToAction("Applications");
        }

        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> Offers()
        {
            var user = await _userManager.GetUserAsync(User);
            var offers =
                await _offerManager.GetOffersByOrganization(await _organizationManager.GetOrganizationFromUserAsync(user));
            var viewModel = new OffersViewModel()
            {
                Offers = offers
            };
            return View(viewModel);
        }
        
        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> Applications()
        {
            var user = await _userManager.GetUserAsync(User);
            var org = await _organizationManager.GetOrganizationFromUserAsync(user);
            var offers = await _offerManager.GetOffersByOrganizationId(org.Id);
            var applications = new List<ApplicationWithDetails>();
            foreach (var offer in offers)
            {
                var apps = await _applicationManager.GetAllApplicationsByOfferId(offer.Id);
                foreach (var app in apps)
                {
                    applications.Add(new ApplicationWithDetails()
                    {
                        Application = app,
                        Offer = offer,
                        User = await _applicationManager.GetUserByApplicationId(app.Id)
                    });
                }
            }
            
            var viewModel = new ApplicationsViewModel()
            {
                Applications = applications
            };
            return View(viewModel);
        }

        [Authorize(Policy = PolicyTypes.Organization.Remove)]
        [InOrganization]
        public async Task<IActionResult> ManageOrganization()
        {
            return View();
        }

        [Authorize(Policy = PolicyTypes.Organization.ManageApplications)]
        [InOrganization]
        public async Task<IActionResult> ShowApplication(string id)
        {
            var applicationWithDetails = new ApplicationWithDetails();
            var application = await _applicationManager.GetApplicationById(id);
            var offer = await _applicationManager.GetOfferByApplicationId(application.Id);
            var user = await _applicationManager.GetUserByApplicationId(application.Id);
            applicationWithDetails.Application = application;
            applicationWithDetails.Offer = offer;
            applicationWithDetails.User = user;
            var viewmodel = new ApplicationWithDetailsModel()
            {
                ApplicationWithDetails = applicationWithDetails
            };
            return View(viewmodel);
        }
        

        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }
        
        [HttpPost]
        [InOrganization]
        public async Task<JsonResult> GetOffers()
        {
            var user = await _userManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var offers = await _offerManager.GetOffersByOrganization(organization);
            return Json(offers);
        }
        
        [HttpPost]
        [InOrganization]
        public async Task<JsonResult> GetApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            var org = await _organizationManager.GetOrganizationFromUserAsync(user);
            var offers = await _offerManager.GetOffersByOrganizationId(org.Id);
            var applications = new List<ToViewApplicationWithDetails>();
            foreach (var offer in offers)
            {
                var apps = await _applicationManager.GetAllApplicationsByOfferId(offer.Id);
                foreach (var app in apps)
                {
                    applications.Add(new ToViewApplicationWithDetails()
                    {
                        Application = app,
                        Offer = offer,
                        User = UserManagerExtensions.TransformToViewUser(await _applicationManager.GetUserByApplicationId(app.Id), offer.Role)
                    });
                }
            }
            return Json(applications);
        }
        
        [HttpPost]
        [InOrganization]
        public async Task<JsonResult> GetUsers()
        {
            var user = await _userManager.GetUserAsync(User);
            var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
            var users = await _organizationManager.GetAllUsersFromOrganizationAsyncByViewModel(organization.Name);
            var transformedUsers = users.Select(u => UserManagerExtensions.TransformToViewUser(u.User, u.Roles.FirstOrDefault())).ToList();
            return Json(transformedUsers);
        }

    }
}