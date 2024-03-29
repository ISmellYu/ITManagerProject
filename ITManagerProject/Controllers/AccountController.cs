﻿using System;
using System.Threading.Tasks;
using ITManagerProject.Models;
using ITManagerProject.Validators;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITManagerProject.Controllers;

public class AccountController : Controller
{
    private UserManager<User> UserManager { get; }
    private SignInManager<User> SignInManager { get; }
    private ILogger<AccountController> Logger { get; }
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
    {
        UserManager = userManager;
        SignInManager = signInManager;
        Logger = logger;
    }
        
    public async Task<IActionResult> Register()
    {
        if (SignInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        await SignInManager.SignOutAsync();
        return View();
    }
        
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerModel, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = new User()
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Country = registerModel.Country,
                City = registerModel.City,
                Address = registerModel.Address,
                PostCode = registerModel.PostalCode,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var result = await UserManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                Logger.LogInformation($"Zarejestrowano uzytkownika o nicku: {user.UserName}");
                await SignInManager.SignInAsync(user, false);
                if (returnUrl == null)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                returnUrl ??= Url.Content("~/");
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == nameof(IdentityErrorDescriber.DuplicateUserName))
                    continue;
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View();
    }
        
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        if (SignInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        await SignInManager.SignOutAsync();
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var user = await UserManager.FindByNameAsync(loginModel.Email);

            if (user != null)
            {
                var result = await SignInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

                if (result.Succeeded)
                {
                    Logger.LogInformation($"Zalogowano uzytkownika o nicku: {user.UserName}");
                    returnUrl ??= Url.Content("~/");
                    return LocalRedirect(returnUrl);
                }
                    
            }
            ModelState.AddModelError(string.Empty, "Nieprawidlowe dane logowania");
        }
            
        return View();
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Overview()
    {
        return View();
    }
        
}