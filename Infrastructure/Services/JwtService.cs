using Infrastructure.Entities;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
    }


    public async Task<string> GenerateTokenAsync(string email)
    {
        try
        {
            var existingUser = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (existingUser?.Email is not null)
            {
                var token = GetToken(existingUser.Email);
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

    public string GetToken(string email, string? role = null)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, email),
                new(ClaimTypes.Email, email)
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token generation failed.");
            return null!;
        }
    }
}
