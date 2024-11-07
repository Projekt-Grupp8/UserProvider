using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context, ILogger<UserService> logger, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, ServiceBusHandler serviceBusHandler)
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ServiceBusHandler _serviceBusHandler = serviceBusHandler;

    public async Task<ResponseResult> CreateUserAsync(SignUpUser model)
    {

        const string STANDARD_ROLE = "User";
        try
        {
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return ResponseFactory.Exists();
            }

            var user = UserFactory.Create(model);
            if (user == null)
            {
                return ResponseFactory.InternalError("Mapping failed.");
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ResponseFactory.InternalError("Test");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, STANDARD_ROLE);
            await _serviceBusHandler.SendServiceBusMessageAsync(model.Email);
            if (!roleResult.Succeeded)
            {
                return ResponseFactory.InternalError("Adding role failed.");
            }

            return ResponseFactory.Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<CreateUserSync> Registration failed.");
            return ResponseFactory.InternalError("Try catch");
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

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Sign in failed for user {Email} {Password}: {Reason}", user.Email, user.Password, result.ToString());
                return ResponseFactory.Error();
            }

            var token = await _jwtService.GenerateTokenAsync(user.Email);
            if (token is null)
            {
                return ResponseFactory.InternalError("Couldn't generate JWT token.");
            }

            return ResponseFactory.Ok(new { user.Email, Token = token }, "Succeeded");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<SignInUserAsync> Sign in failed.");
            return ResponseFactory.InternalError("Try catch");
        }
    }

    public async Task<bool> IsUserVerifiedAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null && user.IsVerified;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<IsUserVerifiedAsync> Couldn't verify user.");
            return false;
        }
    }

    public async Task<ResponseResult> GetAllUsersAsync()
    {
        try
        {
            var userList = await _userManager.Users.ToListAsync();
            var users = UserFactory.Create(userList);
            if (users != null)
            {
                return ResponseFactory.Ok(users);
                //return users.Count > 0
                //    ? ResponseFactory.Ok(users)
                //    : ResponseFactory.NotFound("No users found");
            }
            return ResponseFactory.NotFound();
        }
        catch (Exception)
        {
            _logger.LogError("<GetAllUsersAsync> Couldn't get all users");
            return ResponseFactory.InternalError();
        }
    }
}
