using System;

namespace ITManagerProject.Models
{
    public class Organization
    {
        public Organization()
        {
            
        }

        public Organization(string organizationName) : this()
        {
            Name = organizationName;
            NormalizedName = Name.ToUpper();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public override string ToString()
        {
            return Name;
        }
    }
}