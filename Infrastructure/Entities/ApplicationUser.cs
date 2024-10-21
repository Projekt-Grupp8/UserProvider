using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ApplicationUser  : IdentityUser
{
    [Required]
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;
    [Required]
    [ProtectedPersonalData]
    public string LastName { get; set;} = null!;
    [ProtectedPersonalData]
    public GenderType? Gender { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsExternalAccount { get; set; } = false;
    public bool IsDarkMode { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public DateTime? BirthDate { get; set; }
    public DateTime Created {  get; set; }
    public DateTime Updated { get; set; }
    public DateTime LastLoginDate {  get; set; }
}

public enum GenderType
{
    Male,
    Female,
    Other,
    PreferNotToSay
}
