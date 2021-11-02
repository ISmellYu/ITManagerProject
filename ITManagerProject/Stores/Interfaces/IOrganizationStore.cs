using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ITManagerProject.Stores.Interfaces
{
    public interface IOrganizationStore<TOrganization> : IDisposable where TOrganization : class
    {
        
    }
}