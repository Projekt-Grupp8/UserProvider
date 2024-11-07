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

    public static ApplicationUser Create(UpdateAdmin model)
    {
        try
        {
            return new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
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
                Updated = DateTime.UtcNow,
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"<AdminFactory> ApplicationUser Create:: ERROR: {ex.Message}");
            return null!;
        }
    }

    public static Admin Create(ApplicationUser entity, List<string> roles)
    {
        try
        {
            return new Admin
            {
                Email = entity.Email!,
                FirstName = entity.FirstName!,
                LastName = entity.LastName!,
                Created = entity.Created,
                Updated = entity.Updated,
                Roles = roles
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"<AdminFactory> ApplicationUser Create:: ERROR: {ex.Message}");
            return null!;
        }
    }

    public static ApplicationUser Update(ApplicationUser existingUser, UpdateAdmin model)
    {
        if (model.Email != existingUser.Email)
        {
            existingUser.Email = model.Email;
            existingUser.UserName = model.Email;
        }

        existingUser.FirstName = model.FirstName;
        existingUser.LastName = model.LastName;
        existingUser.Updated = DateTime.UtcNow;

        return existingUser;
    }

   
}
