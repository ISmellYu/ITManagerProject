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
        [StringLength(20, ErrorMessage = "Haslo musi miec przynajmniej 6 znakow oraz maksymalnie 20", MinimumLength = 6)]
        [Display(Name = "Haslo")]
        public string Password { get; set; }
    }
}