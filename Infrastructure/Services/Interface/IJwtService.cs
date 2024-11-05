namespace Infrastructure.Services.Interface
{
    public interface IJwtService
    {
        string GetToken(string email, string? role = null);
    }
}