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
    private readonly AdminCrudService _adminCrudService;
    private readonly UserService _userService;

    public AdminCrudController(UserService userService, AdminCrudService adminCrudService)
    {
        _userService = userService;
        _adminCrudService = adminCrudService;
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
    public async Task<IActionResult> GetOneAdmin()
    {
        // TODO Emma
        return new OkResult();
    }

    [HttpGet]
    [Route("/getalladmin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        var adminList = await _adminCrudService.GetAllAdmin();
        if (adminList.ContentResult is not null)
        {
            return Ok(adminList.ContentResult);
        }

        return NotFound();
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
