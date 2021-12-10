using System;
using ITManagerProject.Models.Interfaces;

namespace ITManagerProject.Models.Base;

public class IdentityUserOrganization<TKey> : IUserOrganization<TKey> where TKey : IEquatable<TKey>
{
    public TKey OrganizationId { get; set; }
    public TKey UserId { get; set; }
}