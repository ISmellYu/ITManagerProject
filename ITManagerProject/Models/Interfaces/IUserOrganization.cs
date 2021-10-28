using System;

namespace ITManagerProject.Models.Interfaces
{
    public interface IUserOrganization<TKey> where TKey : IEquatable<TKey>
    {
        TKey OrganizationId { get; set; }
        TKey UserId { get; set; }
    }
}