namespace Infrastructure.Models;

public class SignUpUser
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool TermsConfirmed { get; set; } = false;
}
