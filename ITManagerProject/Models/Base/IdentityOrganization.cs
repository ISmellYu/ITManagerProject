using System;
using ITManagerProject.Models.Interfaces;

namespace ITManagerProject.Models.Base
{
    public class IdentityOrganization<TKey> : IOrganization<TKey> where TKey : IEquatable<TKey>
    {
        public IdentityOrganization()
        {

        }

        public IdentityOrganization(string organizationName) : this()
        {
            Name = organizationName;
        }

        public TKey Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public override string ToString()
        {
            return Name;
        }
    }
}