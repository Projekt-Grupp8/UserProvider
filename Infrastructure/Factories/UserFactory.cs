using Infrastructure.Entities;
using Infrastructure.Models;
using System.Diagnostics;

namespace Infrastructure.Factories;

public class UserFactory
{
    // Entitet => Model
    public static ApplicationUser Create(SignUpUser model)
    {
        return model == null
            ? throw new ArgumentNullException(nameof(model))
            : new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = null,
                LastName = null,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                IsVerified = false,
                IsSubscribed = false,
                IsDarkMode = false,
                BirthDate = null,
                ProfileImageUrl = null,
                Gender = null
            };
    }

    public static List<User> Create(List<ApplicationUser> userList)
    {
        try
        {
            if (userList == null || userList.Count == 0)
            {
                return null!;
            }

            return userList.Select(user => new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            }).ToList();
        }
        catch (Exception)
        {
            return null!;
        }
    }
}
