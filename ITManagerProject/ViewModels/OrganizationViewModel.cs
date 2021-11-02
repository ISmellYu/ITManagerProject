using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels
{
    public class OrganizationViewModel
    {
        [Required(ErrorMessage = "Nazwa organizacja nie moze byc pusta!")]
        public string Name { get; set; }
    }
}