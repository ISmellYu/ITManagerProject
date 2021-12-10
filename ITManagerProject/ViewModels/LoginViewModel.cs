using System.ComponentModel.DataAnnotations;
using ITManagerProject.ViewModels.Interfaces;

namespace ITManagerProject.ViewModels;

public class LoginViewModel : ILoginViewModel
{
    [Required(ErrorMessage = "Email nie moze byc pusty!")]
    [EmailAddress(ErrorMessage = "Wpisales zly format emaila!")]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Zly format emaila")]
    public string Email { get; set; }
        
    [Required(ErrorMessage = "Haslo nie moze byc puste!")]
    [DataType(DataType.Password, ErrorMessage = "")]
    [Display(Name = "Haslo")]
    public string Password { get; set; }
}