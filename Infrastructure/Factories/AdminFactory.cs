using Infrastructure.Entities;
using Infrastructure.Models;
using System.Diagnostics;

namespace Infrastructure.Factories;

public class AdminFactory
{
    public static ApplicationUser Create(RegisterAdmin model)
    {
        try
        {
            return new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"<AdminFactory> ApplicationUser Create:: ERROR: {ex.Message}");
            return null!;
        }
    }

    public static Admin Create(ApplicationUser entity)
    {
        try
        {
            return new Admin
            {
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"<AdminFactory> ApplicationUser Create:: ERROR: {ex.Message}");
            return null!;
        }
    }
}
