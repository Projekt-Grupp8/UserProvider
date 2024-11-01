using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AdminCrudService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminCrudService> _logger;
    private readonly RoleManager<ApplicationUser> _roleManager;

    public AdminCrudService(UserManager<ApplicationUser> userManager, ILogger<AdminCrudService> logger, RoleManager<ApplicationUser> roleManager)
    {
        _userManager = userManager;
        _logger = logger;
        _roleManager = roleManager;
    }

    public async Task<ResponseResult> CreateAdminAsync(RegisterAdmin model)
    {
        // TODO Emma
        try
        {
            var findAdmin = ExistsAsync(model.Email);
            var body = AdminFactory.Create(model);
            if (findAdmin.IsCompletedSuccessfully)
            {
                await _userManager.CreateAsync(body);
                return ResponseFactory.Ok(body);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR:: AdminCrudService; CreateAdminAsync()");
        }

        return ResponseFactory.Error("Something went wrong, try again later");
    }

    public async Task<ResponseResult> GetOneAdminAsync(string email)
    {
        // TODO Emma
        try
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result is null 
                ? ResponseFactory.NotFound(email) 
                : ResponseFactory.Ok(AdminFactory.Create(result));

            //var findAdmin = ExistsAsync(model.Email);

            //var admin = AdminFactory.Create(model); // gör enbart om användaren 
            //if (findAdmin.IsCompletedSuccessfully)
            //{
            //    var result = await _userManager.FindByEmailAsync(model.Email); // gör väl samma som existsAsync?

            //    return ResponseFactory.Ok(admin);
            //}
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR:: AdminCrudService; GetAdminByIdAsync");
        }

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
                if (roles.Any(role => adminRoles.Contains(role)))
                {
                    adminUsers.Add(user);
                }
            }
            return adminUsers.Count > 0 ? ResponseFactory.Ok(adminUsers) : ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> GetAllAdmin catched an error: {Exception}", ex.Message);
            return ResponseFactory.InternalError();
        }
    }

    public async Task<ResponseResult> UpdateAdminAsync(RegisterAdmin model)
    {
		// TODO Emma
		try
		{
			var findAdmin = ExistsAsync(model.Email);
			var body = AdminFactory.Create(model);
			if (findAdmin is null)
			{
				await _userManager.UpdateAsync(body);
			}

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "ERROR:: AdminCrudService; UpdateAdminAsync()");
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
        try
        {
            var result = await _userManager.FindByEmailAsync(email);

            if (result is null)
            {
                _logger.LogError("Could not find {email}", email);
                return null!;
            }
            return ResponseFactory.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR:: AdminCrudService; ExistsAsync()");
            return null!;
        }
    }
}
