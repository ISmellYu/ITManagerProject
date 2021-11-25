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
        
        [Required(ErrorMessage = "Panstwo nie moze byc puste!")]
        [DataType(DataType.Text)]
        [Display(Name = "Panstwo")]
        public string Country { get; set; }
        
        [Required(ErrorMessage = "Miasto nie moze byc puste!")]
        [DataType(DataType.Text)]
        [Display(Name = "Miasto")]
        public string City { get; set; }
        
        [Required(ErrorMessage = "Adres nie moze byc pusty!")]
        [DataType(DataType.Text)]
        [Display(Name = "Adres")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Kod pocztowy nie moze byc puste!")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Kod pocztowy")]
        public string PostalCode { get; set; }
    }
}