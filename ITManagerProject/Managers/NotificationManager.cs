using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers;

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

    public async Task<bool> AddNotification(string title, string body, int organizationId, int userId)
    {
        ThrowIfDisposed();

        var notification = new Notification
        {
            Title = title,
            Body = body,
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var organizationNotification = new OrganizationNotification
        {
            OrganizationId = organizationId,
            NotificationId = notification.Id,
            AuthorId = userId
                
        };

        _context.OrganizationNotifications.Add(organizationNotification);
        await _context.SaveChangesAsync();

        return true;
    }
        
    public async Task<bool> RemoveNotification(int notificationId)
    {
        ThrowIfDisposed();

        var notification = await Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification == null)
        {
            return false;
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        return true;
    }
        
    public async Task<bool> CheckIfExists(int notificationId)
    {
        ThrowIfDisposed();

        return await Notifications.AnyAsync(n => n.Id == notificationId);
    }
        
    public async Task<List<Notification>> GetNotifications(int organizationId)
    {
        ThrowIfDisposed();

        var organizationNotifications = await OrganizationNotifications.Where(on => on.OrganizationId == organizationId).ToListAsync();

        var notifications = new List<Notification>();

        foreach (var organizationNotification in organizationNotifications)
        {
            var notification = await Notifications.FirstOrDefaultAsync(n => n.Id == organizationNotification.NotificationId);
            notifications.Add(notification);
        }

        return notifications;
    }
        
    public async Task<List<Notification>> GetNotifications(int organizationId, int userId)
    {
        ThrowIfDisposed();

        var organizationNotifications = await OrganizationNotifications.Where(on => on.OrganizationId == organizationId && on.AuthorId == userId).ToListAsync();

        var notifications = new List<Notification>();

        foreach (var organizationNotification in organizationNotifications)
        {
            var notification = await Notifications.FirstOrDefaultAsync(n => n.Id == organizationNotification.NotificationId);
            notifications.Add(notification);
        }

        return notifications;
    }

    public async Task<List<Notification>> GetNotificationsByUserId(int userId)
    {
        ThrowIfDisposed();

        var organizationNotifications =
            await OrganizationNotifications.Where(on => on.AuthorId == userId).ToListAsync();

        var notifications = new List<Notification>();

        foreach (var organizationNotification in organizationNotifications)
        {
            var notification =
                await Notifications.FirstOrDefaultAsync(n => n.Id == organizationNotification.NotificationId);
            notifications.Add(notification);
        }

        return notifications;
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