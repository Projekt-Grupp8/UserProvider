using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, DataContext context, ILogger<UserService> logger, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, IServiceBusHandler serviceBusHandler)
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;

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
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ResponseFactory.InternalError("Test");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, STANDARD_ROLE);

            if (!roleResult.Succeeded)
            {
                return ResponseFactory.InternalError("Adding role failed.");
            }

            await _serviceBusHandler.SendServiceBusMessageAsync(model.Email);

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
            _logger.LogError("< UserService > IsUserVerifiedAsync catched an error: {Exception}", ex.Message);
            return false;
        }
    }

    public async Task<ResponseResult> GetOneUserAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) 
            {
                return ResponseFactory.NotFound();
            }

            var hasUserRole = await _userManager.GetRolesAsync(user);
            if (hasUserRole.Contains("User"))
            {
                return ResponseFactory.Ok(user);
            }
            else
            {
                return ResponseFactory.Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("<UserService> GetOneUserAsync catched an error: {Exception}", ex.Message);
            return ResponseFactory.InternalError();
        }
    }

    public async Task<ResponseResult> GetAllUsersAsync()
    {
        try
        {
            var userRoleUsers = await _userManager.GetUsersInRoleAsync("User");
            var users = UserFactory.Create(userRoleUsers);

            if (users != null && users.Count > 0)
            {
                return ResponseFactory.Ok(users);
            }

            return ResponseFactory.NotFound("No users available");
        }
        catch (Exception ex)
        {
            _logger.LogError("<UserService> GetAllUsersAsync catched an error: {Exception}", ex.Message);
            return ResponseFactory.InternalError();
        }
    }

    public async Task<ResponseResult> UpdateUserAsync(User model)
    {
        try
        {
            // Kontrollerar att användaren faktiskt existerar i databasen. 
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user is null)
            {
                return ResponseFactory.NotFound($"No user with email: {model.Email} exists.");
            }

            // Mappar om UpdateAdmin till en ApplicationUser (entitet). 
            var updateUser = UserFactory.Update(user, model);

            // Updaterar med nya användaruppgifter.
            // Identity hanterar att det inte sker en konflikt om det är den aktuella användaren som försöker uppdatera sin egen e-postadress.
            // Skulle det däremot existera en annan användare med samma inskickade e-post, så kommer uppdateringen inte att lyckas.
            var result = await _userManager.UpdateAsync(updateUser);

            if (result.Succeeded)
            {
                // Mappar om ApplicationUser till en passande modell för att säkerställa att känsliga fält (som passwordHash) inte exponeras.
                return ResponseFactory.Ok(UserFactory.Update(updateUser)); //Retur utan känslig fält.
            }

            return ResponseFactory.InternalError();
        }
        catch (Exception ex)
        {
            _logger.LogError("<UserService> UpdateUserAsync catched an error: {Exception}", ex.Message);
            return null!;
        }
    }

    public async Task<bool> DeleteUserAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperUser") || roles.Contains("Admin"))
            {
                return false;
            }

            await _userManager.DeleteAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("<UserService> DeleteUserAsync catched an error: {Exception}", ex.Message);
            return false;
        }
    }
}
