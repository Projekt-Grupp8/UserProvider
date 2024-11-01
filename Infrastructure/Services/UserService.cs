using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context, ILogger<UserService> logger, SignInManager<ApplicationUser> signInManager, JwtService jwtService, ServiceBusHandler serviceBusHandler) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly JwtService _jwtService = jwtService;
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

            await _serviceBusHandler.SendServiceBusMessageAsync(model.Email);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return ResponseFactory.InternalError();
            }
            await _userManager.AddToRoleAsync(user, STANDARD_ROLE);
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

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Sign in failed for user {Email} {Password}: {Reason}", user.Email, user.Password, result.ToString());
                return ResponseFactory.Error();
            }

            var token = await GenerateTokenAsync(user.Email);
            if (token is null)
            {
                return ResponseFactory.InternalError("Couldn't generate JWT token.");
            }

            return ResponseFactory.Ok(new { user.Email, Token = token }, "Succeeded");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<SignInUserAsync> Sign in failed.");
            return ResponseFactory.InternalError();
        }
    }

    public async Task<string> GenerateTokenAsync(string email)
    {
        try
        {
            var existingUser = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (existingUser?.Email is not null)
            {
                var token = _jwtService.GetToken(existingUser.Email);
                return token;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<GenerateTokenAsync> Failed generating token.");
            return null!;
        }
        return null!;
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
}
