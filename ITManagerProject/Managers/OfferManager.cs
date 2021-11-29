using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers
{
    public class OfferManager : IDisposable
    {
        private readonly UserAppContext _context;
        private readonly OrganizationManager<Organization> _organizationManager;
        
        private IQueryable<Offer> Offers => _context.Offers.AsQueryable().AsNoTracking();
        private IQueryable<OrganizationOffer> OrganizationOffers => _context.OrganizationOffers.AsQueryable().AsNoTracking();
        
        private bool _disposed = false;

        public OfferManager(UserAppContext context, OrganizationManager<Organization> organizationManager)
        {
            _organizationManager = organizationManager;
            _context = context;
        }
        
        public async void AddOffer(Offer offer, int orgId)
        {
            ThrowIfDisposed();
            
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();
            _context.OrganizationOffers.Add(new OrganizationOffer()
            {
                OfferId = offer.Id,
                OrganizationId = orgId
            });
            await _context.SaveChangesAsync();
        }
        
        public async void DeleteOffer(Offer offer)
        {
            ThrowIfDisposed();

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
        }
        
        public async void UpdateOffer(Offer offer)
        {
            ThrowIfDisposed();

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Offer>> GetAllOffers()
        {
            ThrowIfDisposed();

            return await Offers.ToListAsync();
        }
        
        public async Task<Offer> GetOfferById(int id)
        {
            ThrowIfDisposed();

            return await Offers.FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<Offer> GetOfferById(string id)
        {
            ThrowIfDisposed();

            return await Offers.FirstOrDefaultAsync(p => p.Id == Convert.ToInt32(id));
        }

        public async Task<List<Offer>> GetOffersByOrganization(Organization org)
        {
            ThrowIfDisposed();

            var iqOffers = Offers.Where(p => p.Company.ToUpper() == org.NormalizedName);
            return await iqOffers.ToListAsync();
        }
        
        public async Task<bool> OfferExists(int id)
        {
            ThrowIfDisposed();

            return await Offers.AnyAsync(p => p.Id == id);
        }
        
        public async Task<bool> OfferExists(Offer offer)
        {
            ThrowIfDisposed();

            return await Offers.AnyAsync(p => p.Id == offer.Id);
        }
        
        public async Task<Organization> GetOrganizationByOffer(Offer offer)
        {
            ThrowIfDisposed();

            var orgOffer = await OrganizationOffers.FirstOrDefaultAsync(p => p.OfferId == offer.Id);
            return await _organizationManager.GetOrganizationAsync(orgOffer.OrganizationId);
        }
        
        public async Task<List<Offer>> GetOffersByOrganizationId(int id)
        {
            ThrowIfDisposed();

            var orgOffers = await OrganizationOffers.Where(p => p.OrganizationId == id).ToListAsync();
            var offers = new List<Offer>();
            foreach (var orgOffer in orgOffers)
            {
                var offer = await Offers.FirstOrDefaultAsync(p => p.Id == orgOffer.OfferId);
                offers.Add(offer);
            }
            return offers;
        }


        public OrganizationOffer CreateOrganizationOffer(Organization organization, Offer offer)
        {
            ThrowIfDisposed();

            return new OrganizationOffer()
            {
                OrganizationId = organization.Id,
                OfferId = offer.Id
            };
        }
        
        public OrganizationOffer CreateOrganizationOffer(int orgId, int offerId)
        {
            ThrowIfDisposed();

            return new OrganizationOffer()
            {
                OrganizationId = orgId,
                OfferId = offerId
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}