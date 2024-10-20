using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context, ILogger<UserService> logger, SignInManager<ApplicationUser> signInManager)
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<ResponseResult> CreateUserAsync(SignUpUser model)
    {
        try
        {
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return ResponseFactory.Exists();
            }

            var user = UserFactory.Create(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return ResponseFactory.InternalError();
            }

            return ResponseFactory.Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<CreateUserSync> Registration failed.");
            return ResponseFactory.InternalError();
        }
    }

    public async Task<ResponseResult> SignInUserAsync(SignInUser user)
    {
        try
        {
            if (!_userManager.Users.Any(x => x.Email == user.Email))
            {
                return ResponseFactory.NotFound();
            }

            // Eventuellt lägga till en RememberMe på isPersistent? 
            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return ResponseFactory.InvalidCredentials();
            }

            return ResponseFactory.Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<SignInUserAsync> Sign in failed.");
            return ResponseFactory.InternalError();
        }
    }
}
