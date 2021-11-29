using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ITManagerProject.ViewModels
{
    public class EditViewModel
    {
        public User User { get; set; }
        [Required]
        [Range(0, 18)]
        public string RoleId { get; set; }
        [Required]
        [MaxLength(8, ErrorMessage = "Mozesz podawac 8 znakow liczb")]
        [Range(1, 99999999)]
        public int Salary { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}