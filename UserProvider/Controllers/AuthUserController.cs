using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserProvider.Helpers;
using UserProvider.Models;

namespace UserProvider.Controllers;

public class AuthUserController(DataContext context) : Controller
{
    private readonly DataContext _context = context;

    public async Task<IActionResult> Register(User user)
    {
        if(!Validation.ValidateEmail(user.Email))
        {
            return Conflict();
        }

        if (!ModelState.IsValid)
        {
            return Conflict();
        }


        try
        {
            if (!await _context.Users.AnyAsync(x => x.Email == user.Email))
            {
                // Factory call (omvandlar model => entitet)
                // Kalla på repo och skicka in entiteten för att skapas.

                return Created("", user);
            }
            return View();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR:: Registration failed: {ex.Message}");
            return BadRequest();
        }
    }
}
