namespace Infrastructure.Models;

public class VerifyUser
{
    public string Email { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
