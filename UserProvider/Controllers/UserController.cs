using Azure;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace UserProvider.Controllers;

[ApiController]
public partial class UserController : ControllerBase
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

    [HttpPut]
    [Route("/updateuser")]
    public async Task<IActionResult> UpdateUser(User model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _userService.UpdateUserAsync(model);
        if (response.ContentResult is not null)
        {
            return Ok(response.ContentResult);
        }

        return NotFound($"Couldn't find user with email: {model.Email}");
    }

    [HttpDelete]
    [Route("/deleteuser")]
    public async Task<IActionResult> DeleteUser([FromQuery]string email)
    {
        if (string.IsNullOrEmpty(email) || !MyRegex().IsMatch(email))
        {
            return BadRequest("Invalid email format.");
        }

        var deleted = await _userService.DeleteUserAsync(email);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound($"No user with email: {email}");
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex MyRegex();
}
