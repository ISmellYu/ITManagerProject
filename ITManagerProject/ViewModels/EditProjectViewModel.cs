using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManagerProject.ViewModels;

public class EditProjectViewModel
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Status { get; set; }
    [Required]
    public string Priority { get; set; }
    
    public List<SelectListItem> Statuses { get; set; }
    public List<SelectListItem> Priorities { get; set; }
}