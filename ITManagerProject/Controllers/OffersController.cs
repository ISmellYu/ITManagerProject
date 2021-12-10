using System;
using System.Threading.Tasks;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers;

[Authorize]
public class OffersController : Controller
{
    private readonly OfferManager _offerManager;
    private readonly ApplicationManager _applicationManager;
    private readonly UserManager<User> _userManager;
    private readonly OrganizationManager<Organization> _organizationManager;

    public OffersController(OfferManager offerManager, ApplicationManager applicationManager, 
        UserManager<User> userManager, OrganizationManager<Organization> organizationManager)
    {
        _organizationManager = organizationManager;
        _userManager = userManager;
        _applicationManager = applicationManager;
        _offerManager = offerManager;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }
        
    [HttpGet]
    [Route("/Offers/{id:int}")]
    public async Task<IActionResult> ShowOffer(int id)
    {
        if (!(await _offerManager.OfferExists(id)))
        {
            return RedirectToAction("Index");
        }

        var offer = await _offerManager.GetOfferById(id);
        return View(offer);
    }
        
    [HttpGet]
    [Route("Offers/Apply/{id:int}")]
    public async Task<IActionResult> Apply(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (!(await _offerManager.OfferExists(id)) || 
            await _applicationManager.CheckIfApplicationExists(id, user.Id) || 
            await _organizationManager.CheckIfInAnyOrganizationAsync(user))
        {
            return RedirectToAction("Index");
        }

        var offer = await _offerManager.GetOfferById(id);
        var applyViewModel = new ApplyViewModel()
        {
            ApplicationViewModel = new ApplicationViewModel(),
            Offer = offer,
        };
        return View(applyViewModel);
    }
        
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Offers/Apply/{id:int}")]
    public async Task<IActionResult> Apply(int id, ApplicationViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (!(await _offerManager.OfferExists(id)) || 
            await _applicationManager.CheckIfApplicationExists(id, user.Id) || 
            await _organizationManager.CheckIfInAnyOrganizationAsync(user))
        {
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Apply", new { id });
        }

            
        var offer = await _offerManager.GetOfferById(id);
        await _applicationManager.AddApplication(new Application()
        {
            Cv = viewModel.Cv
        },offer.Id, user.Id);

        return RedirectToAction("Success");
    }
        
    public async Task<IActionResult> Success()
    {
        return View();
    }
}