using System;

namespace ITManagerProject.Models.Interfaces;

public interface IOrganization<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
    string Name { get; set; }
    string NormalizedName { get; set; }
    string ConcurrencyStamp { get; set; }
    string ToString();
}