using System.ComponentModel.DataAnnotations;

namespace ITManagerProject.ViewModels
{
    public class LoginViewModel
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