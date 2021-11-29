using System;
using System.Threading;
using System.Threading.Tasks;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITManagerProject.HelperTypes
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> ChangeSalary(this UserManager<User> userManager, User user, int salary)
        {
            user.Salary = salary;
            await userManager.UpdateAsync(user);
            return true;
        }
    }
}