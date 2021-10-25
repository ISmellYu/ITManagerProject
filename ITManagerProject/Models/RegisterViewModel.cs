using System.ComponentModel.DataAnnotations;

namespace ITManagerProject.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Haslo")]
        public string Password { get; set; }
    }
}