using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManagerProject.ViewModels;

public class AddUserModel
{
    [Required(ErrorMessage = "Musisz wybrac uzytkownika!")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "Musisz wybrac role!")]
    public string RoleId { get; set; }
    [Required(ErrorMessage = "Musisz podac place!")]
    [DataType(DataType.Currency)]
    [Range(1, 99999999, ErrorMessage = "Podaj prawidlową wartosc!")]
    public int Salary { get; set; }
        
    public IEnumerable<SelectListItem> RolesList { get; set; }
    public IEnumerable<SelectListItem> UsersList { get; set; }
}