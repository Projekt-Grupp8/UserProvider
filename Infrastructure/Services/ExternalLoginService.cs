using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Infrastructure.Services;

public class ExternalLoginService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ExternalLoginService> logger, JwtService jwtService)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<ExternalLoginService> _logger = logger;
    private readonly JwtService _jwtService = jwtService;


    public async Task<(bool Success, string Message, string? Token)> HandleExternalLoginAsync()
    {
        var userInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (userInfo == null)
        {
            _logger.LogError("External login information is null.");
            return (false, "External login failed.", null);
        }

        var userEntity = new ApplicationUser
        {
            FirstName = userInfo.Principal.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = userInfo.Principal.FindFirstValue(ClaimTypes.Surname)!,
            Email = userInfo.Principal.FindFirstValue(ClaimTypes.Email)!,
            UserName = userInfo.Principal.FindFirstValue(ClaimTypes.Email)!,
            IsExternalAccount = true
        };

        var user = await _userManager.FindByEmailAsync(userEntity.Email);
        if (user == null)
        {
            var result = await _userManager.CreateAsync(userEntity);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return (false, "User creation failed.", null);
            }

            user = await _userManager.FindByEmailAsync(userEntity.Email);
        }

        if (user!.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
        {
            user.FirstName = userEntity.FirstName;
            user.LastName = userEntity.LastName;
            user.Email = userEntity.Email;

            await _userManager.UpdateAsync(user);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        var token = _jwtService.GetToken(user.Email);
        return (true, "External login successful.", token);
    }
}
