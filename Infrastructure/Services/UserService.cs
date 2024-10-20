using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context, ILogger<UserService> logger)
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly DataContext _context = context;

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
}
