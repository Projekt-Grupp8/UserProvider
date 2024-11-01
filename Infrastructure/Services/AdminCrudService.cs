using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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
            _logger.LogError(ex,"ERROR:: AdminCrudService; CreateAdminAsync()");
        }
        
        return ResponseFactory.Error("Something went wrong, try again later");
    }

    public async Task<ResponseResult> GetOneAdminAsync(string email, RegisterAdmin model)
    {
		// TODO Emma
		try
		{
			var findAdmin = ExistsAsync(email);
            var admin = AdminFactory.Create(model); // gör enbart om användaren 
            if (findAdmin.IsCompletedSuccessfully)
            {
                var result = await _userManager.FindByEmailAsync(email); // gör väl samma som existsAsync?
                
                return ResponseFactory.Ok(admin);
			}
			
            
            

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "ERROR:: AdminCrudService; GetAdminByIdAsync");
		}

		return null!;

		
    }

    public async Task<ResponseResult> GetAllAdmin()
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> UpdateAdminAsync(RegisterAdmin model)
    {
		// TODO Emma
		try
		{
			var findAdmin = ExistsAsync(model.Email);
			var body = AdminFactory.Create(model);
			if (findAdmin is not null)
			{
				await _userManager.UpdateAsync(body);
			}

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "ERROR:: AdminCrudService; UpdateAdminAsync()");
		}

		return null!;
	}

    public async Task<ResponseResult> DeleteAdminAsync(string email)
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> ExistsAsync(string email)
    {
        // TODO
        try
        {
            var result = await _userManager.FindByEmailAsync(email);

			if (result is null)
            {
                _logger.LogError($"Could not find {email}", email);
            }
            return ResponseFactory.Ok();
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "ERROR:: AdminCrudService; ExistsAsync()");
        }

        return null!;
    }
}
