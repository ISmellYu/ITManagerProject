﻿namespace ITManagerProject.ViewModels.Interfaces;

public interface IRegisterViewModel
{
    string Email { get; set; }
    string Password { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
}