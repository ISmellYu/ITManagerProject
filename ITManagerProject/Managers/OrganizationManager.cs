using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.HelperTypes;
using ITManagerProject.Models;
using ITManagerProject.Models.Interfaces;
using ITManagerProject.Stores.Interfaces;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ITManagerProject.Managers;

public class OrganizationManager<TOrganization> : IDisposable where TOrganization : class
{
    private readonly UserAppContext _dbContext;
    private IQueryable<Organization> Organizations => _dbContext.Organizations.AsQueryable().AsNoTracking();
    private IQueryable<UserOrganization> UserOrganizations => _dbContext.UserOrganizations.AsQueryable().AsNoTracking();
    public readonly UserManager<User> UserManager;
    public readonly RoleManager<Role> RoleManager;

    private bool _disposed;

    public OrganizationManager(UserAppContext dbContext, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        RoleManager = roleManager;
        UserManager = userManager;
        _dbContext = dbContext;
    }

    public async Task CreateAsync(string organizationName)
    {
        ThrowIfDisposed();
        var exists = await CheckIfOrganizationExistsAsync(organizationName);
        if (!exists)
        {
            //Console.WriteLine("Creating org");
            _dbContext.Organizations.Add(new Organization()
            {
                Name = organizationName,
                NormalizedName = organizationName.ToUpper()
            });
            await _dbContext.SaveChangesAsync();
        }
    }
        
    public async Task<bool> RemoveAsync(string organizationName)
    {
        ThrowIfDisposed();
        var exists = await CheckIfOrganizationExistsAsync(organizationName);
            
        if (!exists) return false;

        var org = Organizations.First(p => p.NormalizedName == organizationName.ToUpper());
        var users = await GetAllUsersFromOrganizationAsync(org.Name);
        _dbContext.Organizations.Remove(org);
        await _dbContext.SaveChangesAsync();
        foreach (var user in users)
        {
            await UserManager.RemoveFromRolesAsync(user, RoleTypesString.AllRolesAvailable);
            await UserManager.ChangeSalary(user, 0);
        }
        return true;
    }
        
    public async Task<bool> AddToOrganizationAsync(User user, string organizationName, int salary = 0, string role = null)
    {
        ThrowIfDisposed();
            
        var exists = await CheckIfOrganizationExistsAsync(organizationName);
            
        if (!exists) return false;
        var normalizedOrganizationName = Normalize(organizationName);
        if (await CheckIfInAnyOrganizationAsync(user))
        {
            return false;
        }

        var org = await GetOrganizationAsync(normalizedOrganizationName);

        _dbContext.UserOrganizations.Add(CreateUserOrganization(user, org));
        //var alreadyInRole = await UserManager.IsInRoleAsync(user, role);
        if (role != null)
        {
            await UserManager.AddToRoleAsync(user, role);
            await UserManager.ChangeSalary(user, salary);
        }
        else
        {
            await UserManager.AddToRoleAsync(user, RoleTypesString.CEO);
            await UserManager.ChangeSalary(user, 100);
        }
            
        await _dbContext.SaveChangesAsync();
            
        return true;
    }
        
    public async Task<bool> AddToOrganizationAsync(User user, Organization organization, int salary = 0, string role = null)
    {
        ThrowIfDisposed();

        var organizationName = organization.Name;
        var exists = await CheckIfOrganizationExistsAsync(organizationName);
            
        if (!exists) return false;
        var normalizedOrganizationName = Normalize(organizationName);
        if (await CheckIfInAnyOrganizationAsync(user))
        {
            return false;
        }

        _dbContext.UserOrganizations.Add(CreateUserOrganization(user, organization));
        if (role != null)
        {
            await UserManager.AddToRoleAsync(user, role);
            await UserManager.ChangeSalary(user, salary);
        }
        else
        {
            await UserManager.AddToRoleAsync(user, RoleTypesString.CEO);
            await UserManager.ChangeSalary(user, 100);
        }

        await _dbContext.SaveChangesAsync();
            
        return true;
    }
        
    public async Task<bool> RemoveFromOrganizationAsync(User user, string organizationName)
    {
        ThrowIfDisposed();
            
        var exists = await CheckIfOrganizationExistsAsync(organizationName);
            
        if (!exists) return false;
        var normalizedOrganizationName = Normalize(organizationName);
        if (!await CheckIfInAnyOrganizationAsync(user))
        {
            return false;
        }

        var normalizedOrgName = Normalize(organizationName);
        var org = await GetOrganizationAsync(organizationName);

        if (org != null)
        {
            if (await CheckIfInOrganizationAsync(user, normalizedOrganizationName))
            {
                var userOrg = await GetUserOrganizationAsync(user, org);
                if (userOrg != null)
                {
                    _dbContext.UserOrganizations.Remove(userOrg);
                    await _dbContext.SaveChangesAsync();
                    await UserManager.RemoveFromRoleAsync(user, (await UserManager.GetRolesAsync(user))[0]);
                    user.Salary = 0;
                    await UserManager.UpdateAsync(user);
                    return true;
                }
            }
        }
        return false;
    }

    public async Task<bool> ChangeSalary(User user, int salary)
    {
        ThrowIfDisposed();

        return await UserManager.ChangeSalary(user, salary);
    }

    public async Task<bool> CheckIfOrganizationExistsAsync(string organizationName)
    {
        ThrowIfDisposed();
        var exists = await Organizations.AnyAsync(p => p.NormalizedName == Normalize(organizationName));
        return exists;
    }
        
    public async Task<bool> CheckIfInOrganizationAsync(User user, string normalizedOrganizationName)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(normalizedOrganizationName))
        {
            throw new ArgumentException();
        }

        var org = await GetOrganizationAsync(normalizedOrganizationName);
        if (org != null)
        {
            var userOrg = await GetUserOrganizationAsync(user.Id, org.Id);
            return userOrg != null;
        }

        return false;
    }
        
    public async Task<bool> CheckIfInOrganizationAsync(User user, Organization organization)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(organization.NormalizedName))
        {
            throw new ArgumentException();
        }

        var org = await GetOrganizationAsync(organization.NormalizedName);
        if (org != null)
        {
            var userOrg = await GetUserOrganizationAsync(user.Id, org.Id);
            return userOrg != null;
        }

        return false;
    }

    public async Task<string> GetRoleForUser(User user)
    {
        ThrowIfDisposed();
        var ifInOrg = await CheckIfInAnyOrganizationAsync(user);
        if (!ifInOrg)
            return null;

        var roles = await UserManager.GetRolesAsync(user) as List<string>;
        var role = roles?.FirstOrDefault();
        return role;
    }

    public async Task<bool> CheckIfInAnyOrganizationAsync(User user)
    {
        ThrowIfDisposed();
        var exists = await UserOrganizations.AnyAsync(p => p.UserId == user.Id);
        return exists;
    }

    public async Task<Organization> GetOrganizationFromUserAsync(User user)
    {
        ThrowIfDisposed();
        var userOrg = await UserOrganizations.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (userOrg == null)
            return null;
        return await Organizations.FirstOrDefaultAsync(p => p.Id == userOrg.OrganizationId);
    }
        
    public async Task<Organization> GetOrganizationAsync(int orgId)
    {
        ThrowIfDisposed();
        var org = await Organizations.FirstOrDefaultAsync(p => p.Id == orgId);
        return org;
    }

    private async Task<Organization> GetOrganizationAsync(string normalizedName)
    {
        ThrowIfDisposed();
        return await Organizations.SingleOrDefaultAsync(p => p.NormalizedName == normalizedName, CancellationToken.None);
    }
        
    private async Task<UserOrganization> GetUserOrganizationAsync(int userId, int orgId)
    {
        ThrowIfDisposed();
        return await _dbContext.UserOrganizations.FindAsync(userId, orgId).AsTask();
    }
        
    private async Task<UserOrganization> GetUserOrganizationAsync(User user, Organization org)
    {
        ThrowIfDisposed();
        return await _dbContext.UserOrganizations.FindAsync(user.Id, org.Id).AsTask();
    }

    public async Task<List<UserOrganizationViewModel>> GetAllUsersFromOrganizationAsyncByViewModel(string organizationName)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(organizationName))
        {
            throw new ArgumentException();
        }

        var normalizedName = Normalize(organizationName);
        var exists = await CheckIfOrganizationExistsAsync(normalizedName);

        if (!exists) return null;

        var org = await GetOrganizationAsync(normalizedName);

        var listOfUsers = new List<UserOrganizationViewModel>();
        if (org != null)
        {
            var userIds = await GetUserIdsFromOrganization(org);
            foreach (var id in userIds)
            {
                var user = await UserManager.FindByIdAsync(id.ToString());
                var userRoles = await UserManager.GetRolesAsync(user);

                var viewModel = new UserOrganizationViewModel()
                {
                    User = user,
                    Roles = userRoles as List<string>
                };
                    
                listOfUsers.Add(viewModel);
            }
        }

        return listOfUsers;
    }

    public async Task<List<User>> GetAllUsersFromOrganizationAsync(string organizationName)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(organizationName))
        {
            throw new ArgumentException();
        }

        var normalizedName = Normalize(organizationName);
        var exists = await CheckIfOrganizationExistsAsync(normalizedName);

        if (!exists) return null;

        var org = await GetOrganizationAsync(normalizedName);

        var listOfUsers = new List<User>();
        if (org != null)
        {
            var userIds = await GetUserIdsFromOrganization(org);
            foreach (var id in userIds)
            {
                var user = await UserManager.FindByIdAsync(id.ToString());
                listOfUsers.Add(user);
            }
        }

        return listOfUsers;
    }



    private async Task<User> GetUserById(int id)
    {
        return await UserManager.Users.FirstOrDefaultAsync(p => p.Id == id);
    }

    private async Task<List<int>> GetUserIdsFromOrganization(Organization org)
    {
        return UserOrganizations.Where(p => p.OrganizationId == org.Id).Select(organization => organization.UserId).ToList();
    }

    // Get number of users in organization
    public async Task<int> GetNumberOfUsersInOrganization(string organizationName)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(organizationName))
        {
            throw new ArgumentException();
        }

        var normalizedName = Normalize(organizationName);
        var exists = await CheckIfOrganizationExistsAsync(normalizedName);

        if (!exists) return 0;

        var org = await GetOrganizationAsync(normalizedName);

        if (org != null)
        {
            var userIds = await GetUserIdsFromOrganization(org);
            return userIds.Count;
        }

        return 0;
    }
        
        
    private string Normalize(string txt)
    {
        return txt.ToUpper();
    }

    private UserOrganization CreateUserOrganization(User user, Organization organization)
    {
        return new UserOrganization()
        {
            UserId = user.Id,
            OrganizationId = organization.Id
        };
    }
        
    private UserOrganization CreateUserOrganization(int userId, int orgId)
    {
        return new UserOrganization()
        {
            UserId = userId,
            OrganizationId = orgId
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