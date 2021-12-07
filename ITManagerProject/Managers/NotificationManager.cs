using System;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers
{
    public class NotificationManager : IDisposable
    {
        private OrganizationManager<Organization> _organizationManager;
        private UserAppContext _context;

        private IQueryable<Notification> Notifications => _context.Notifications.AsNoTracking();

        private IQueryable<OrganizationNotification> OrganizationNotifications =>
            _context.OrganizationNotifications.AsNoTracking();

        private bool _disposed = false;

        public NotificationManager(OrganizationManager<Organization> organizationManager, UserAppContext context)
        {
            _context = context;
            _organizationManager = organizationManager;
        }

        public async Task<bool> AddNotification()
        {
            ThrowIfDisposed();
            return false;
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