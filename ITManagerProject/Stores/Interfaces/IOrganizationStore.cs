using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ITManagerProject.Stores.Interfaces
{
    public interface IOrganizationStore<TOrganization, TUser> : IDisposable where TOrganization : IDisposable where TUser : class
    {
        Task CreateOrganizationAsync(string organizationName, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string organizationName, CancellationToken cancellationToken);
        Task<bool> ExistsByIdAsync(string organizationId, CancellationToken cancellationToken);
        Task<bool> ExistsByUserAsync(TUser user, CancellationToken cancellationToken);
        Task<bool> ChangeOrganizationNameAsync(string oldOrganizationName, CancellationToken cancellationToken);
        Task AddToOrganizationAsync(string organizationName, TUser user, CancellationToken cancellationToken);
        Task RemoveFromOrganizationAsync(string organizationName, TUser user, CancellationToken cancellationToken);
        Task AddToOrganizationRangeAsync(string organizationName, List<TUser> users, CancellationToken cancellationToken);
        Task RemoveFromOrganizationRangeAsync(string organizationName, List<TUser> users,
            CancellationToken cancellationToken);
        Task<List<TUser>> GetUsersFromOrganizationByIdAsync(string organizationId, CancellationToken cancellationToken);
        Task<List<TUser>> GetUsersFromOrganizationByNameAsync(string organizationName, CancellationToken cancellationToken);
    }
}