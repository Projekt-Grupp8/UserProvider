using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AdminCrudService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminCrudService> _logger;
    public AdminCrudService(UserManager<ApplicationUser> userManager, ILogger<AdminCrudService> logger, RoleManager<ApplicationUser> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<ResponseResult> CreateAdminAsync(RegisterAdmin model)
    {
        // TODO
        return null!;
    }

    public async Task<ResponseResult> GetAdminByIdAsync(string email)
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> GetAllAdmin()
    {
        try
        {
            var userList = await _userManager.Users.ToListAsync();

            var adminRoles = new[] { "admin", "superuser" };
            var adminUsers = new List<ApplicationUser>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if(roles.Any(role => adminRoles.Contains(role)))
                {
                    adminUsers.Add(user);
                }
            }
            return userList.Count > 0 ? ResponseFactory.Ok(userList) : ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> GetAllAdmin catched an error: {Exception}", ex.Message);
            return ResponseFactory.InternalError();
        }
    }

    public async Task<ResponseResult> UpdateAdminAsync(RegisterAdmin model)
    {
        // TODO

        return null!;
    }

    public async Task<bool> DeleteAdminAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> DeleteAdminAsync catched an error: {Exception}", ex.Message);
            return false;
        }
    }

    public async Task<ResponseResult> ExistsAsync(string email)
    {
        // TODO

        return null!;
    }
}
