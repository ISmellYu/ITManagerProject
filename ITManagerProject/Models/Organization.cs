using System;

namespace ITManagerProject.Models
{
    public class Organization<TKey> where TKey : IEquatable<TKey>
    {
        public Organization()
        {
            
        }

        public Organization(string organizationName) : this()
        {
            
        }
        
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}