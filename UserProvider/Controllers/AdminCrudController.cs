using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
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
    private readonly AdminCrudService _adminService;

	public AdminCrudController(UserService userService, AdminCrudService adminService)
	{
		_userService = userService;
		_adminService = adminService;
	}


	[HttpPost]
    [Route("/createadmin")]
    public async Task<IActionResult> CreateAdmin(RegisterAdmin model)
    {
        // TODO Emma
        try
        {
            if (!ModelState.IsValid)
            {
                await _adminService.CreateAdminAsync(model);
            }

            return new OkResult();
        }
        catch (Exception ex)
        {
            return new BadRequestResult();
        }

    }

    [HttpGet]
    [Route("/getadminbyid")]
    public async Task<IActionResult> GetOneAdmin()
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
