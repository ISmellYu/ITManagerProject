using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.Managers;

public class ProjectManager : IDisposable
{
    private readonly OrganizationManager<Organization> _organizationManager;
    private readonly UserAppContext _context;

    private IQueryable<Project> Projects => _context.Projects.AsNoTracking();
    private IQueryable<ProjectOrganization> ProjectOrganizations => _context.ProjectOrganizations.AsNoTracking();
    
    private bool _disposed = false;

    public ProjectManager(OrganizationManager<Organization> organizationManager, UserAppContext context)
    {
        _context = context;
        _organizationManager = organizationManager;
    }

    public async Task<bool> CreateProject(Project project, int organizationId)
    {
        ThrowIfDisposed();
        try
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            
            await _context.ProjectOrganizations.AddAsync(new ProjectOrganization()
            {
                ProjectId = project.Id,
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
    
    public async Task<bool> DeleteProject(int projectId)
    {
        ThrowIfDisposed();
        try
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return false;
            }
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateProject(Project project)
    {
        ThrowIfDisposed();
        try
        {
            var projectToUpdate = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
            if (projectToUpdate == null)
            {
                return false;
            }
            projectToUpdate.Name = project.Name;
            projectToUpdate.Description = project.Description;
            projectToUpdate.Status = project.Status;
            projectToUpdate.Priority = project.Priority;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<Project> GetProject(int projectId)
    {
        ThrowIfDisposed();
        return await Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    }
    
    public async Task<IEnumerable<Project>> GetProjects()
    {
        ThrowIfDisposed();
        return await Projects.ToListAsync();
    }
    
    public async Task<IEnumerable<Project>> GetProjectsByOrganization(int organizationId)
    {
        ThrowIfDisposed();
        var ids = ProjectOrganizations.Where(p => p.OrganizationId == organizationId).Select(p => p.ProjectId);
        return await Projects.Where(p => ids.Contains(p.Id)).ToListAsync();
    }
    
    public async Task<Organization> GetOrganizationFromProject(int projectId)
    {
        ThrowIfDisposed();
        var projectOrganization = await ProjectOrganizations.FirstOrDefaultAsync(p => p.ProjectId == projectId);
        var org = await _organizationManager.GetOrganizationAsync(projectOrganization.OrganizationId);
        return org;
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