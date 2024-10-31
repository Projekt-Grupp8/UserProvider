using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserProvider.Controllers;

[ApiController]
[Authorize("SuperUser")]
public class AdminCrudController : ControllerBase
{
    private readonly UserService _userService;

    public AdminCrudController(UserService userService)
    {
        _userService = userService;
    }


    [HttpPost]
    [Route("/createadmin")]
    public async Task<IActionResult> CreateAdmin()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpGet]
    [Route("/getadminbyid")]
    public async Task<IActionResult> GetAdminById()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpGet]
    [Route("/getalladmin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        // TODO Ted
        return new OkResult();
    }

    [HttpPost]
    [Route("/updateadmin")]
    public async Task<IActionResult> UpdateAdmin()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpPost]
    [Route("/deleteadmin")]
    public async Task<IActionResult> DeleteAdmin()
    {
        // TODO Ted
        return new OkResult();
    }
}
