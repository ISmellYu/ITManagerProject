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
        
        private IQueryable<Application> _applications => _context.Applications.AsQueryable();
        private IQueryable<OfferApplication> _offerApplications => _context.OfferApplications.AsQueryable();
        
        private bool _disposed = false;
        public ApplicationManager(UserAppContext context)
        {
            _context = context;
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
        
        public async Task<bool> RemoveApplication(Application application, int offerId, int userId)
        {
            ThrowIfDisposed();
            if (!(await CheckIfApplicationExists(application)))
            {
                return false;
            }
            
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            var offerApplication = await _offerApplications.FirstOrDefaultAsync(oa => oa.OfferId == offerId && oa.ApplicationId == application.Id);
            _context.OfferApplications.Remove(offerApplication);
            await _context.SaveChangesAsync();
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

        public async Task<Application> GetApplicationById(int id)
        {
            ThrowIfDisposed();
            return await _applications.FirstOrDefaultAsync(p => p.Id == id);
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