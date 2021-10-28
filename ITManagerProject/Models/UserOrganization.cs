using ITManagerProject.Models.Interfaces;

namespace ITManagerProject.Models
{
    public class UserOrganization : IUserOrganization
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
    }
}