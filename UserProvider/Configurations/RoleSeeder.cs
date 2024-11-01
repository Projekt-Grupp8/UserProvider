using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace UserProvider.Configurations;

public static class RoleSeeder
{
    public static async Task SeedSuperAdminAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["SuperUser", "Admin", "User"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

        string adminEmail = configuration["AdminSettings:Email"]!;
        string adminPassword = configuration["AdminSettings:Password"]!;

        if (userManager.Users.All(u => u.UserName != "admin@rika.com"))
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperUser");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                }
            }
        }
    }
}
