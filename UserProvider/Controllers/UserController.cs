using Azure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace UserProvider.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("/getusers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        if (response.StatusCode == Infrastructure.Models.StatusCode.OK)
        {
            return Ok(response.ContentResult);
        }

        return StatusCode((int)response.StatusCode, response.Message);
    }
}
