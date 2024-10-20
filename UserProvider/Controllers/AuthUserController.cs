using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models;
using ResponseStatusCode = Infrastructure.Models.StatusCode;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserProvider.Controllers;

public class AuthUserController(DataContext context, UserManager<ApplicationUser> userManager, UserService userService, ILogger<AuthUserController> logger) : Controller
{
    private readonly ILogger<AuthUserController> _logger = logger;
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

            return result.StatusCode switch
            {
                ResponseStatusCode.OK => Created("Registration succeeded", result.ContentResult),
                ResponseStatusCode.EXISTS => Conflict("The user with this e-mail address already exists"),
                ResponseStatusCode.ERROR => BadRequest("Please provide all required information"),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<Controller 'Register' Called> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return BadRequest("An unexpected internal error occurred. Please try again later.");
        }
    }

    public async Task<IActionResult> SignInUser(SignInUser model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.SignInUserAsync(model);

            return result.StatusCode switch
            {
                ResponseStatusCode.OK => Created("Sign in succeeded", result.ContentResult),
                ResponseStatusCode.EXISTS => Conflict("No user found with this e-mail address"),
                ResponseStatusCode.INVALID_CREDENTIALS => BadRequest("Invalid credentials. Please check your input."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<Controller 'SignInUser' Called> :: Sign in failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return BadRequest("An unexpected internal error occurred. Please try again later.");
        }
    }
}
