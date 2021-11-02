using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using ITManagerProject.Models.Interfaces;
using ITManagerProject.Stores.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ITManagerProject.Managers
{
    public class OrganizationManager<TOrganization> : IDisposable where TOrganization : class
    {
        private readonly UserAppContext _dbContext;
        private IQueryable<Organization> Organizations => _dbContext.Organizations.AsQueryable();
        private IQueryable<UserOrganization> UserOrganizations => _dbContext.UserOrganizations.AsQueryable();
        public readonly UserManager<User> UserManager;
        
        private bool _disposed;

        public OrganizationManager(UserAppContext dbContext, UserManager<User> userManager)
        {
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
            _dbContext.Organizations.Remove(org);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> AddToOrganizationAsync(User user, string organizationName)
        {
            ThrowIfDisposed();
            
            var exists = await CheckIfOrganizationExistsAsync(organizationName);
            
            if (!exists) return false;
            var normalizedOrganizationName = Normalize(organizationName);
            if (await CheckIfInAnyOrganizationAsync(user))
            {
                return false;
            }

            var org = await FindOrganizationAsync(normalizedOrganizationName);

            _dbContext.UserOrganizations.Add(CreateUserOrganization(user, org));
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
            var org = await FindOrganizationAsync(organizationName);

            if (org != null)
            {
                if (await CheckIfInOrganizationAsync(user, normalizedOrganizationName))
                {
                    var userOrg = await FindUserOrganizationAsync(user, org);
                    if (userOrg != null)
                    {
                        _dbContext.UserOrganizations.Remove(userOrg);
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
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

            var org = await FindOrganizationAsync(normalizedOrganizationName);
            if (org != null)
            {
                var userOrg = await FindUserOrganizationAsync(user.Id, org.Id);
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

            var org = await FindOrganizationAsync(organization.NormalizedName);
            if (org != null)
            {
                var userOrg = await FindUserOrganizationAsync(user.Id, org.Id);
                return userOrg != null;
            }

            return false;
        }


        public async Task<bool> CheckIfInAnyOrganizationAsync(User user)
        {
            var exists = await UserOrganizations.AnyAsync(p => p.UserId == user.Id);
            return exists;
        }

        public async Task<Organization> GetOrganization(User user)
        {
            var userOrg = await UserOrganizations.FirstOrDefaultAsync(p => p.UserId == user.Id);
            return await Organizations.FirstOrDefaultAsync(p => p.Id == userOrg.OrganizationId);
        }

        private async Task<Organization> FindOrganizationAsync(string normalizedName)
        {
            return await Organizations.SingleOrDefaultAsync(p => p.NormalizedName == normalizedName, CancellationToken.None);
        }
        
        private async Task<UserOrganization> FindUserOrganizationAsync(int userId, int orgId)
        {
            return await _dbContext.UserOrganizations.FindAsync(new object[userId, orgId]).AsTask();
        }
        
        private async Task<UserOrganization> FindUserOrganizationAsync(User user, Organization org)
        {
            return await _dbContext.UserOrganizations.FindAsync(new object[user.Id, org.Id]).AsTask();
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
}