using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models;

public class RegisterAdmin
{
    [ProtectedPersonalData]
    public string Email { get; set; } = null!;

    [ProtectedPersonalData]
    public string Password { get; set; } = null!;

    [ProtectedPersonalData]
    public string? FirstName { get; set; }

    [ProtectedPersonalData]
    public string? LastName { get; set; }

    public string? ProfileImageUrl { get; set; }
    public bool IsDarkMode { get; set; } = false;
    public DateTime? BirthDate { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime LastLoginDate { get; set; }
}
