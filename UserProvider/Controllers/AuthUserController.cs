using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UserProvider.Controllers;

public class AuthUserController(DataContext context, UserManager<ApplicationUser> userManager, UserService userService) : Controller
{
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly UserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Register(SignUpUser model)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.CreateUserAsync(model);

            if (result.StatusCode is Infrastructure.Models.StatusCode.OK)
            {
                return Created("Registration succeeded", result.ContentResult);
            }

            return Conflict(result.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR:: Registration failed: {ex.Message}");
            return BadRequest();
        }
    }
}
