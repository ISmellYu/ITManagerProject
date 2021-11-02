using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels
{
    public class RegisterViewModel : IRegisterViewModel
    {
        [Required(ErrorMessage = "Email nie moze byc pusty!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Nieprawidlowy email!")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Haslo nie moze byc puste!")]
        [DataType(DataType.Password)]
        [Display(Name = "Haslo")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Imie nie moze byc puste!")]
        [DataType(DataType.Text)]
        [Display(Name = "Imie")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Nazwisko nie moze byc puste!")]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        
    }
}