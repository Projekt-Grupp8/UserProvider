using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace Infrastructure.Services;

public class AdminService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, ILogger<AdminService> logger)
{
    private readonly ILogger<AdminService> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ResponseResult> SignIn(SignInAdmin admin)
    {
        try
        {
            var role = _userManager.Users.FirstOrDefault(x => x.Email == admin.Email);
            
            
            if (!_userManager.Users.Any(x => x.Email == admin.Email))
            {
                return ResponseFactory.NotFound();
            }

            var result = await _signInManager.PasswordSignInAsync(admin.Email, admin.Password, false, false);

            if (!result.Succeeded)
            {
                return ResponseFactory.Error();
            }

            var token = _jwtService.GetToken(admin.Email);

            if (token is null)
            {
                return ResponseFactory.Error("Couldnt generate token.");
            }

            return ResponseFactory.Ok(new { admin.Email, token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ADMINSERVICE <SignIn> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return ResponseFactory.InternalError();
        }
    }
}
