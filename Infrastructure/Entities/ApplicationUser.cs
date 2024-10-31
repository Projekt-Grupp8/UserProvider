using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Entities;

public class ApplicationUser  : IdentityUser
{
    [ProtectedPersonalData]
    public string? FirstName { get; set; }

    [ProtectedPersonalData]
    public string? LastName { get; set;}

    [ProtectedPersonalData]
    public GenderType? Gender { get; set; }

    public string? ProfileImageUrl { get; set; }
    public bool IsExternalAccount { get; set; } = false;
    public bool IsDarkMode { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool IsSubscribed { get; set; } = false;
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
