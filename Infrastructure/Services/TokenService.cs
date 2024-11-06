using Infrastructure.Entities;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<TokenService> _logger;

    public TokenService(UserManager<ApplicationUser> userManager, IJwtService jwtService, ILogger<TokenService> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
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
}
