using System.Threading.Tasks;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers
{
    [Authorize]
    public class OffersController : Controller
    {
        private readonly OfferManager _offerManager;
        private string xd;

        public OffersController(OfferManager offerManager)
        {
            _offerManager = offerManager;
        }
        // GET
        public IActionResult Index()
        {
            xd = "hahga";
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
        [Route("/Offers/Apply/{id:int}")]
        public async Task<IActionResult> Apply(int id)
        {
            if (!(await _offerManager.OfferExists(id)))
            {
                return RedirectToAction("Index");
            }

            var offer = await _offerManager.GetOfferById(id);
            var org = await _offerManager.GetOrganizationByOffer(offer);
            var viewModel = new ApplyViewModel()
            {
                Application = new Application(),
                Offer = offer,
                Organization = org
            };
            return View(viewModel);
        }
    }
}