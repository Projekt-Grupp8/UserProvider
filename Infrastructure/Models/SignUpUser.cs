using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class SignUpUser
{
    [Required(ErrorMessage = "You must provide a user name.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "You must provide a password.")]
    [StringLength(64, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "Password doesn't match.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "You must provide an email.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail address")]
    public string Email { get; set; } = null!;

    [CheckboxRequired(ErrorMessage = "You must accept terms and conditions.")]
    public bool TermsConfirmed { get; set; } = false;
}
