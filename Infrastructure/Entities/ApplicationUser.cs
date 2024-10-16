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
    public string Gender { get; set; } = null!;
    public string Age { get; set; } = null!;
    public string ProfileImageUrl { get; set; } = null!;
    public DateTime? Created {  get; set; }
    public DateTime? Updated { get; set;}
    public bool IsExternalAccount { get; set; } = false;
    public bool IsDarkMode { get; set; } = false;
}
