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

        public virtual TKey Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public override string ToString()
        {
            return Name;
        }
    }
}