using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password, ErrorMessage = "")]
        [Display(Name = "Haslo")]
        public string Password { get; set; }
    }
}