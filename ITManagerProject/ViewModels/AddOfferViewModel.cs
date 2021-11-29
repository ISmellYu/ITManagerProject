using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManagerProject.ViewModels
{
    public class AddOfferViewModel
    {
        [Required(ErrorMessage = "Musisz podać nazwę oferty")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Musisz podać opis oferty")]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Musisz wybrac role")]
        public string RoleId { get; set; }
        [Required(ErrorMessage = "Musisz wybrac lokalizacje")]
        [DataType(DataType.Text)]
        public string Location { get; set; }
        [Required(ErrorMessage = "Musisz podac place")]
        [DataType(DataType.Currency)]
        [Range(1, 99999999, ErrorMessage = "Podaj wartosc w zakresie od 1 do 99999999")]
        public int Salary { get; set; }
        
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}