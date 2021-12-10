using System.Collections.Generic;
using ITManagerProject.Models;

namespace ITManagerProject.ViewModels;

public class UserOrganizationViewModel
{
    public User User { get; set; }
    public List<string> Roles { get; set; }
}