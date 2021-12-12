using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers;

public class EventManager : IDisposable
{
    private readonly UserAppContext _context;
    private readonly OrganizationManager<Organization> _organizationManager;

    private IQueryable<Event> Events => _context.Events.AsNoTracking();
    private IQueryable<EventOrganization> EventOrganizations => _context.EventOrganizations.AsNoTracking();

    private bool _disposed = false;
    public EventManager(UserAppContext context, OrganizationManager<Organization> organizationManager)
    {
        _context = context;
        _organizationManager = organizationManager;
    }

    public async Task<bool> AddEvent(Event @event, int organizationId)
    {
        try
        {
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
            
            await _context.EventOrganizations.AddAsync(new EventOrganization()
            {
                EventId = @event.Id,
                OrganizationId = organizationId
            });
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateEvent(Event @event)
    {
        try
        {
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteEvent(int id)
    {
        try
        {
            var @event = await Events.FirstOrDefaultAsync(e => e.Id == id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<Event> GetEvent(int id)
    {
        return await Events.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<IEnumerable<Event>> GetAllEvents()
    {
        return await Events.ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllEvents(int organizationId)
    {
        var ids = await EventOrganizations.Where(u => u.OrganizationId == organizationId).ToListAsync();
        var events = new List<Event>();
        foreach (var id in ids)
        {
            events.Add(await Events.FirstOrDefaultAsync(e => e.Id == id.EventId));
        }
        return events;
    }
    
    public async Task<IEnumerable<Event>> GetAllEvents(int organizationId, DateTime startDate, DateTime endDate)
    {
        var ids = EventOrganizations.Where(u => u.OrganizationId == organizationId);
        var events = new List<Event>();
        foreach (var id in ids)
        {
            var @event = await Events.FirstOrDefaultAsync(e => e.Id == id.EventId);
            if (@event.StartDate >= startDate && @event.EndDate <= endDate)
            {
                events.Add(@event);
            }
        }
        return events;
    }
    
    public async Task<IEnumerable<Event>> GetAllEvents(int organizationId, DateTime startDate)
    {
        var ids = EventOrganizations.Where(u => u.OrganizationId == organizationId);
        var events = new List<Event>();
        foreach (var id in ids)
        {
            var @event = await Events.FirstOrDefaultAsync(e => e.Id == id.EventId);
            if (@event.StartDate >= startDate)
            {
                events.Add(@event);
            }
        }
        return events;
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