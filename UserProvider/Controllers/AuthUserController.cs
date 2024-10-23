using Infrastructure.Entities;
using Infrastructure.Models;
using ResponseStatusCode = Infrastructure.Models.StatusCode;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserProvider.Filters;

namespace UserProvider.Controllers;

[ApiController]
public class AuthUserController(UserManager<ApplicationUser> userManager, UserService userService, ILogger<AuthUserController> logger, SignInManager<ApplicationUser> signInManager, JwtService jwtService) : Controller
{
    private readonly ILogger<AuthUserController> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly UserService _userService = userService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly JwtService _jwtService = jwtService;

    [HttpPost("/register")]
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
                _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected internal error occurred. Please try again later.")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<Register> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return BadRequest("An unexpected internal error occurred. Please try again later.");
        }
    }

    [HttpPost("/signin")]
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
                ResponseStatusCode.OK => Ok(result),
                ResponseStatusCode.EXISTS => Conflict("No user found with this e-mail address"),
                ResponseStatusCode.INVALID_CREDENTIALS => BadRequest("Invalid credentials. Please check your input."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected internal error occurred. Please try again later.")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<SignInUser> :: Sign in failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return BadRequest("An unexpected internal error occurred. Please try again later.");
        }
    }

    [Authorize]
    [HttpPost]
    [Route("/logout")]
    public async Task<IActionResult> LogOut()
    {
        try
        {
            Response.Cookies.Delete("AccessToken");
            await _signInManager.SignOutAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<LogOut> :: Sign out failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [ApiKey]
    [HttpPost]
    [Route("/token")]
    public IActionResult GetToken(SignInUser model)
    {
        if (ModelState.IsValid)
        {
            var tokenString = _jwtService.GetToken(model.Email);
            return Ok(tokenString);
        }
        return Unauthorized();
    }
}
