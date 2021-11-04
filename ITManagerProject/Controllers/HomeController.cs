﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.HelperTypes;
using ITManagerProject.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ITManagerProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly OrganizationManager<Organization> _organizationManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, OrganizationManager<Organization> organizationManager, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _organizationManager = organizationManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // var s = new List<string>()
            // {
            //     "CTO", "CIO", "Head of Product", "Product Manager", "VP of Marketing", "Director of Engineering",
            //     "Chief Architect", "Software Architect", "Engineering Project Manager",
            //     "Technical Lead", "Principal Software Engineer", "Senior Software Engineer", "Software Engineer",
            //     "Software Developer", "Junior Software Developer", "Intern Software Developer"
            // };
            //
            // foreach (var g in s)
            // {
            //     await _roleManager.CreateAsync(new Role(g));
            // }
            // await _organizationManager.RoleManager.SeedClaimsForRole("CEO", new List<string>()
            // {
            //     Permissions.Users.Add,
            //     Permissions.Users.Edit,
            //     Permissions.Users.View,
            //     Permissions.Users.Remove,
            //     Permissions.Organization.Remove
            // });
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error" ,new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}