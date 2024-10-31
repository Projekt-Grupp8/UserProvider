using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class SignInUser
{
    [Required(ErrorMessage = "You must provide an email.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "You must provide a password.")]
    [StringLength(64, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
    public string Password { get; set; } = null!;

    [Display(Name = "RememberMe", Order = 2)]
    public bool RememberMe { get; set; }
}
