using System.Collections.Generic;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ITManagerProject.ViewModels
{
    public class EditViewModel
    {
        public User User { get; set; }
        public string RoleId { get; set; }
        public int Salary { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}