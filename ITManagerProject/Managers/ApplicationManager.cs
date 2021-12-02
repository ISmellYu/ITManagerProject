using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers
{
    public class ApplicationManager : IDisposable
    {
        private readonly UserAppContext _context;
        private readonly OrganizationManager<Organization> _organizationManager;
        private readonly OfferManager _offerManager;

        private IQueryable<Application> _applications => _context.Applications.AsQueryable().AsNoTracking();
        private IQueryable<OfferApplication> _offerApplications => _context.OfferApplications.AsQueryable().AsNoTracking();
        
        private bool _disposed = false;
        public ApplicationManager(UserAppContext context, OrganizationManager<Organization> organizationManager, OfferManager offerManager)
        {
            _context = context;
            _organizationManager = organizationManager;
            _offerManager = offerManager;
        }

        public async Task<bool> AddApplication(Application application, int offerId, int userId)
        {
            ThrowIfDisposed();
            if (await CheckIfApplicationExists(offerId, userId))
            {
                return false;
            }
            
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            _context.OfferApplications.Add(new OfferApplication
            {
                OfferId = offerId,
                ApplicationId = application.Id,
                UserId = userId
            });
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> RemoveApplication(Application application)
        {
            ThrowIfDisposed();
            if (!(await CheckIfApplicationExists(application)))
            {
                return false;
            }
            
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            // var offerApplication = await _offerApplications.FirstOrDefaultAsync(oa => oa.OfferId == offerId && oa.ApplicationId == application.Id);
            // _context.OfferApplications.Remove(offerApplication);
            // await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> AcceptApplication(Application application, int offerId, int userId)
        {
            ThrowIfDisposed();
            if (!(await CheckIfApplicationExists(application)))
            {
                return false;
            }

            var organization = await _offerManager.GetOrganizationByOffer(new Offer() { Id = offerId });
            var user = await _organizationManager.UserManager.FindByIdAsync(userId.ToString());
            var offer = await _offerManager.GetOfferById(offerId);
            await _organizationManager.AddToOrganizationAsync(user, organization, offer.Salary, offer.Role);
            //await _organizationManager.ChangeSalary(user, offer.Salary);
            await RemoveApplication(application);
            return true;
        }

        public async Task<bool> CheckIfApplicationExists(int offerId, int userId)
        {
            ThrowIfDisposed();
            return await _offerApplications.AnyAsync(x => x.OfferId == offerId && x.UserId == userId);
        }
        
        public async Task<bool> CheckIfApplicationExists(Application application)
        {
            ThrowIfDisposed();
            return await _offerApplications.AnyAsync(x => x.ApplicationId == application.Id);
        }
        
        public async Task<bool> CheckIfApplicationExists(int applicationId)
        {
            ThrowIfDisposed();
            return await _offerApplications.AnyAsync(x => x.ApplicationId == applicationId);
        }
        
        public async Task<List<Application>> GetAllApplications()
        {
            ThrowIfDisposed();
            return await _applications.ToListAsync();
        }
        
        public async Task<List<Application>> GetAllApplicationsByOfferId(int offerId)
        {
            ThrowIfDisposed();
            var ids = _offerApplications.Where(p => p.OfferId == offerId).Select(p => p.ApplicationId).ToList();
            return await _applications.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
        
        public async Task<List<Application>> GetAllApplicationsByUserId(int userId)
        {
            ThrowIfDisposed();
            var ids = _offerApplications.Where(p => p.UserId == userId).Select(p => p.ApplicationId).ToList();
            return await _applications.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
        
        public async Task<List<Application>> GetApplicationsByOrganizationId(int organizationId)
        {
            ThrowIfDisposed();
            var offers = await _offerManager.GetOffersByOrganizationId(organizationId);
            var offerApps = await _offerApplications.Where(p => offers.Any(x => x.Id == p.OfferId)).ToListAsync();
            var ids = offerApps.Select(p => p.ApplicationId).ToList();
            var applications = await _applications.Where(p => ids.Contains(p.Id)).ToListAsync();
            return applications;
        }

        public async Task<Application> GetApplicationById(int id)
        {
            ThrowIfDisposed();
            return await _applications.FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<Application> GetApplicationById(string id)
        {
            ThrowIfDisposed();
            return await _applications.FirstOrDefaultAsync(p => p.Id == Convert.ToInt32(id));
        }
        
        public async Task<User> GetUserByApplicationId(int applicationId)
        {
            ThrowIfDisposed();
            var offerApplication = await _offerApplications.FirstOrDefaultAsync(p => p.ApplicationId == applicationId);
            return await _organizationManager.UserManager.FindByIdAsync(offerApplication.UserId.ToString());
        }
        
        public async Task<Offer> GetOfferByApplicationId(int applicationId)
        {
            ThrowIfDisposed();
            var offerApplication = await _offerApplications.FirstOrDefaultAsync(p => p.ApplicationId == applicationId);
            return await _offerManager.GetOfferById(offerApplication.OfferId);
        }

        public async Task<bool> RemoveApplications(List<Application> applications)
        {
            ThrowIfDisposed();
            foreach (var application in applications)
            {
                await RemoveApplication(application);
            }
            return true;
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