using Infrastructure.Models;
using Infrastructure.Services;
using ResponseStatusCode = Infrastructure.Models.StatusCode;
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
    [Route("/getoneuser")]
    public async Task<IActionResult> GetOneUser([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email) || !MyRegex().IsMatch(email))
        {
            return BadRequest("Invalid email format.");
        }

        var result = await _userService.GetOneUserAsync(email);
        return result.StatusCode switch
        {
            ResponseStatusCode.OK => Ok(new { status = "success", message = "User found", email = email }),
            ResponseStatusCode.UNAUTHORIZED => Unauthorized(new { status = "error", message = "User does not have the correct role or does not exist" }),
            ResponseStatusCode.NOT_FOUND => NotFound(new { status = "error", message = "User not found" }),
            _ => StatusCode(500, new { status = "error", message = "An unexpected internal error occurred. Please try again later." })
        };
    }

    [HttpGet]
    [Route("/getusers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        if (response.StatusCode == ResponseStatusCode.OK)
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
        return response.StatusCode switch
        {
            ResponseStatusCode.OK => Ok(response.ContentResult),
            ResponseStatusCode.UNAUTHORIZED => Unauthorized(new { status = "error", message = "You do not have permission to update this user." }),
            ResponseStatusCode.NOT_FOUND => NotFound(new { status = "error", message = $"Couldn't find user with email: {model.Email}" }),
            _ => StatusCode(500, new { status = "error", message = "An unexpected error occurred. Please try again later." })
        };
    }

    [HttpDelete]
    [Route("/deleteuser")]
    public async Task<IActionResult> DeleteUser([FromQuery] string email)
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
        return Unauthorized(new { status = "error", message = "You do not have permission to delete this user." });
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex MyRegex();
}
