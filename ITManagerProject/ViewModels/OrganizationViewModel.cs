using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels
{
    public class OrganizationViewModel : IOrganizationViewModel
    {
        [Required]
        [Display(Name = "Nazwa organiacji")]
        public string Name { get; set; }
    }
}