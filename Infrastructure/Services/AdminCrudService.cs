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

    public AdminCrudService(UserManager<ApplicationUser> userManager, ILogger<AdminCrudService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ResponseResult> CreateAdminAsync(RegisterAdmin model)
    {

        const string ADMIN_USER = "Admin";

        try
        {
            var findAdmin = await _userManager.FindByEmailAsync(model.Email!);
            if(findAdmin is not null)
            {

                return ResponseFactory.Exists(model.Email);
            }
            var body = AdminFactory.Create(model);

            await _userManager.CreateAsync(body, model.Password);
            await _userManager.AddToRoleAsync(body, ADMIN_USER);
            return ResponseFactory.Ok("New admin created {body.Email}",body.Email);

        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> CreateAdminAsync catched an error: {Exception}", ex.Message);
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

            
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> GetOneAdminAsync catched an error: {Exception}", ex.Message);
            return ResponseFactory.Error();
			
		}

    }

    public async Task<ResponseResult> GetAllAdmin()
    {
        try
        {
            var userList = await _userManager.Users.ToListAsync();
            var adminRoles = new[] { "Admin", "SuperUser" };
            var adminUsers = new List<ApplicationUser>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(role => adminRoles.Contains(role)))
                {
                    adminUsers.Add(user);
                }
            }

            var adminModels = new List<Admin>();
            foreach (var admin in adminUsers)
            {
                var roles = (await _userManager.GetRolesAsync(admin)).ToList();
                adminModels.Add(AdminFactory.Create(admin, roles));
            }

            return adminModels.Count > 0 ? ResponseFactory.Ok(adminModels) : ResponseFactory.NotFound();
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
            var findAdmin = await _userManager.FindByEmailAsync(model.Email);  //Kollar om mejl finns
			
			if (findAdmin is null)
            {
                return ResponseFactory.NotFound(model.Email);

            }
			var body = AdminFactory.Create(model); //omvandlar registerAdmin till aplicationuser för att kunna updatera i usermanager
			await _userManager.UpdateAsync(body);
            

			return ResponseFactory.Ok(AdminFactory.Create(body)); //Retunerar hel adminObjektet
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> UpdateAdminAsync catched an error: {Exception}", ex.Message);
            return null!;
        }
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
            return ResponseFactory.Ok(email);
        }
        catch (Exception ex)
        {
            _logger.LogError("<AdminCrudService> ExistsAsync catched an error: {Exception}", ex.Message);
            return null!;
        }
    }
}
