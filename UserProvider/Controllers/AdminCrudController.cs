using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;


namespace UserProvider.Controllers;

[ApiController]
//[Authorize(Policy = "AuthenticatedAdmins")]
public partial class AdminCrudController : ControllerBase
{
    private readonly AdminCrudService _adminCrudService;
    private readonly ILogger<AdminCrudController> _logger;

    public AdminCrudController(AdminCrudService adminCrudService, ILogger<AdminCrudController> logger)
    {
        _adminCrudService = adminCrudService;
        _logger = logger;
    }

    [HttpPost]
    [Route("/createadmin")]
    public async Task<IActionResult> CreateAdmin(RegisterAdmin model)
    {
        if (ModelState.IsValid)
        {
            var createAdmin = await _adminCrudService.CreateAdminAsync(model);
            if (createAdmin is not null)
            {
                return Ok(new { userCreated = model.Email });
            }
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("/getadminbyemail")]
    public async Task<IActionResult> GetOneAdmin([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email) || !MyRegex().IsMatch(email))
        {
            return BadRequest("Invalid email format.");
        }

        var getAdmin = await _adminCrudService.GetOneAdminAsync(email);
        if (getAdmin.ContentResult is not null)
        {
            return Ok(new { updatedAdmin = getAdmin.ContentResult });
        }

        return NotFound(new { message = "User not found" });
    }

    [HttpGet]
    [Route("/getalladmin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        try
        {
            var adminList = await _adminCrudService.GetAllAdmin();
            if (adminList.ContentResult is not null)
            {
                return Ok(adminList.ContentResult);
            }

            return NotFound("No admins found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while fetching all admins: {Exception}", ex.Message);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost]
    [Route("/updateadmin")]
    public async Task<IActionResult> UpdateAdmin(UpdateAdmin model)
    {
        if (ModelState.IsValid)
        {
            var updateAdmin = await _adminCrudService.UpdateAdminAsync(model);
            if (updateAdmin.ContentResult is not null)
            {
                return Ok(new { updatedAdmin = updateAdmin.ContentResult });
            }
            return NotFound(new { message = "user not found" });
        }
        return BadRequest();
    }

    [HttpDelete]
    [Route("/deleteadmin")]
    public async Task<IActionResult> DeleteAdmin([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email) || !MyRegex().IsMatch(email))
        {
            return BadRequest("Invalid email format.");
        }

        try
        {
            var deleted = await _adminCrudService.DeleteAdminAsync(email);
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting admin with email { Email}: { Exception}", email, ex.Message);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex MyRegex();
}
