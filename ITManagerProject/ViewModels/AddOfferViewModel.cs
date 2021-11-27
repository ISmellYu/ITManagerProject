using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManagerProject.ViewModels
{
    public class AddOfferViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public int Salary { get; set; }
        
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}