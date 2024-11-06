namespace Infrastructure.Services.Interface
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(string email);
    }
}