
namespace Infrastructure.Services.Interface
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(string email);
        string GetToken(string email, string? role = null);
    }
}