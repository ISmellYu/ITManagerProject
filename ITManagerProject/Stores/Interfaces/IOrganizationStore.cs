using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ITManagerProject.Stores.Interfaces
{
    public interface IOrganizationStore<TOrganization, TUser> : IDisposable where TOrganization : IDisposable where TUser : class
    {
        Task<List<TUser>> GetUsersFromOrganizationByIdAsync(string organizationId, CancellationToken cancellationToken);
        
        Task<List<TUser>> GetUsersFromOrganizationByNameAsync(string organizationName, CancellationToken cancellationToken);
    }
}