using System.ComponentModel.DataAnnotations;

namespace ITManagerProject.ViewModels
{
    public class OrganizationViewModel
    {
        [Required]
        [Display(Name = "Nazwa organiacji")]
        public string Name { get; set; }
    }
}