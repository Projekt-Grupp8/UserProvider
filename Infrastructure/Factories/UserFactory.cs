using Infrastructure.Entities;
using Infrastructure.Models;
using System.Diagnostics;

namespace Infrastructure.Factories;

public class UserFactory
{
    // Entitet => Model
    public static ApplicationUser Create(SignUpUser model)
    {
        try
        {
            return new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ApplicationUser Create:: ERROR: {ex.Message}");
            return null!;
        }
    }
  
}
