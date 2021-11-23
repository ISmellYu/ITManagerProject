using System;
using System.Linq;
using ITManagerProject.Contexts;
using ITManagerProject.Models;

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