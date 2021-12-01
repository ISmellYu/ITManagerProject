using System;
using System.Linq;
using System.Threading.Tasks;
using ITManagerProject.Contexts;
using ITManagerProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ITManagerProject.Managers
{
    //TODO: Override UserManager to add logic for CookieRenewal
    public class CookieRenewManager
    {
        private readonly UserAppContext _context;

        private IQueryable<UserCookieRenew> UserCookieRenews => _context.UserCookieRenews.AsNoTracking();
        public CookieRenewManager(UserAppContext context)
        {
            _context = context;
        }
        
        public async Task<bool> CheckIfCookieRenewExists(string userId)
        {
            var cookieRenew = await UserCookieRenews.AnyAsync(p => p.UserId == Convert.ToInt32(userId));
            return cookieRenew;
        }
        
        public async Task<bool> CheckIfCookieRenewExists(int userId)
        {
            var cookieRenew = await UserCookieRenews.AnyAsync(p => p.UserId == userId);
            return cookieRenew;
        }
        
        public async Task<bool> CheckIfCookieRenewExists(UserCookieRenew userCookieRenew)
        {
            var cookieRenew = await UserCookieRenews.AnyAsync(p => p.UserId == userCookieRenew.UserId);
            return cookieRenew;
        }
        
        public async Task<bool> DeleteCookieRenew(UserCookieRenew userCookieRenew)
        {
            if (await CheckIfCookieRenewExists(userCookieRenew))
            {
                _context.UserCookieRenews.Remove(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> DeleteCookieRenew(int userId)
        {
            if (await CheckIfCookieRenewExists(userId))
            {
                var userCookieRenew = await UserCookieRenews.FirstOrDefaultAsync(p => p.UserId == userId);
                _context.UserCookieRenews.Remove(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> DeleteCookieRenew(string userId)
        {
            if (await CheckIfCookieRenewExists(userId))
            {
                var userCookieRenew = await UserCookieRenews.FirstOrDefaultAsync(p => p.UserId == Convert.ToInt32(userId));
                _context.UserCookieRenews.Remove(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> AddCookieRenew(UserCookieRenew userCookieRenew)
        {
            if (!(await CheckIfCookieRenewExists(userCookieRenew)))
            {
                await _context.UserCookieRenews.AddAsync(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> AddCookieRenew(int userId)
        {
            if (!(await CheckIfCookieRenewExists(userId)))
            {
                var userCookieRenew = new UserCookieRenew
                {
                    UserId = userId
                };
                await _context.UserCookieRenews.AddAsync(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> AddCookieRenew(string userId)
        {
            if (!(await CheckIfCookieRenewExists(userId)))
            {
                var userCookieRenew = new UserCookieRenew
                {
                    UserId = Convert.ToInt32(userId)
                };
                await _context.UserCookieRenews.AddAsync(userCookieRenew);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task<bool> CheckIfShouldRenew(string userId)
        {
            return await CheckIfCookieRenewExists(userId);
        }

        public async Task<bool> CheckIfShouldRenew(int userId)
        {
            return await CheckIfCookieRenewExists(userId);
        }
        
    }
}