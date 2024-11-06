using Infrastructure.Entities;
using Infrastructure.Models;
using ResponseStatusCode = Infrastructure.Models.StatusCode;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserProvider.Filters;
using Infrastructure.Services.Interface;

namespace UserProvider.Controllers;

[ApiController]
public class AuthUserController(UserManager<ApplicationUser> userManager, UserService userService, ILogger<AuthUserController> logger, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, ServiceBusHandler serviceBusHandler) : Controller
{
    private readonly ILogger<AuthUserController> _logger = logger;
    private readonly UserService _userService = userService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ServiceBusHandler _serviceBusHandler = serviceBusHandler;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

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
                ResponseStatusCode.OK => Created("Registration succeeded", new { status = "success", email = model.Email }),
                ResponseStatusCode.EXISTS => Conflict(new { status = "error", message = "The user with this e-mail address already exists" }),
                ResponseStatusCode.ERROR => BadRequest(new { status = "error", message = "Please provide all required information" }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = "An unexpected internal error occurred. Please try again later." })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<Register> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return BadRequest("An unexpected internal error occurred. Please try again later.");
        }
    }

    [HttpPost("/verify")]
    public async Task<IActionResult> VerifyUser(VerifyUser model)
    {
        try
        {
            if (!await _serviceBusHandler.VerifyCodeAsync(model.Email, model.VerificationCode))
            {
                return Unauthorized(new { message = "User verification not completed."});
            }

            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<VerifyUser> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
            return StatusCode(StatusCodes.Status500InternalServerError, "Registration failed due to an internal error");
        }
    }

    [HttpPost("/signin")]
    public async Task<IActionResult> SignInUser(SignInUser model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _userService.IsUserVerifiedAsync(model.Email))
        {
            return Unauthorized(new { message = "User account not verified." });
        }

        try
        {
            var result = await _userService.SignInUserAsync(model);
            return result.StatusCode switch
            {
                ResponseStatusCode.OK => Ok(new { status = "success", data = result }),
                ResponseStatusCode.EXISTS => Conflict(new { status = "error", message = "No user found with this e-mail address" }),
                ResponseStatusCode.UNAUTHORIZED => BadRequest(new { status = "error", message = "Invalid credentials. Please check your input." }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = "An unexpected internal error occurred. Please try again later." })
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
