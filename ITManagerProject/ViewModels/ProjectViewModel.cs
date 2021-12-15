using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManagerProject.ViewModels;

public class ProjectViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Status { get; set; }
    [Required]
    public string Priority { get; set; }
    [Required]
    public string ManagerId { get; set; }

    public IEnumerable<SelectListItem> UsersList { get; set; }
}