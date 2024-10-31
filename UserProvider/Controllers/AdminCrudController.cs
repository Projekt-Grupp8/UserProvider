using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserProvider.Controllers;

[ApiController]
[Authorize("SuperUser")]
public class AdminCrudController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminCrudController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    [Route("/createadmin")]
    public async Task<IActionResult> CreateAdmin()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAdminById()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAdmin()
    {
        // TODO Ted
        return new OkResult();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAdmin()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAdmin()
    {
        // TODO Ted
        return new OkResult();
    }
}
