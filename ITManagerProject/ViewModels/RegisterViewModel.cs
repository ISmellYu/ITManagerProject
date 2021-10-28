using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels
{
    public class RegisterViewModel : IRegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Haslo musi miec przynajmniej 6 znakow oraz maksymalnie 20", MinimumLength = 6)]
        [Display(Name = "Haslo")]
        public string Password { get; set; }
    }
}