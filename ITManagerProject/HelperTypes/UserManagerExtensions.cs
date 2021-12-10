using System;
using System.Threading;
using System.Threading.Tasks;
using ITManagerProject.Models;
using ITManagerProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.HelperTypes;

public static class UserManagerExtensions
{
    public static async Task<bool> ChangeSalary(this UserManager<User> userManager, User user, int salary)
    {
        user.Salary = salary;
        await userManager.UpdateAsync(user);
        return true;
    }
        
    public static ToViewUser TransformToViewUser(User user, string role)
    {
        var userToView = new ToViewUser
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Salary = user.Salary,
            Country = user.Country,
            City = user.City,
            PostCode = user.PostCode,
            Role = role
        };
        return userToView;
    }
}