using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context)
{
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

            return ResponseFactory.Ok(user);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR:: Registration failed: {ex.Message}");
            return ResponseFactory.InternalError();
        }
    }
}
