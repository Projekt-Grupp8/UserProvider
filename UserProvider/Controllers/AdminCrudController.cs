
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;


namespace UserProvider.Controllers;

[ApiController]
//[Authorize(Policy = "AuthenticatedAdmins")]
public class AdminCrudController : ControllerBase
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
                return Ok(model.Email);
            }
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("/getadminbyid")]
    public async Task<IActionResult> GetOneAdmin(string email)
    {
        if (ModelState.IsValid)
        {

            var getAdmin = await _adminCrudService.GetOneAdminAsync(email);
            if (getAdmin.ContentResult is not null)
            {
                return Ok(getAdmin.ContentResult);
            }

            return NotFound($"{email} not found.");
        }

        return BadRequest();

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
                return Ok(updateAdmin.ContentResult);
            }
            return NotFound(model.Email);
        }
        return BadRequest();
    }

    [HttpPost]
    [Route("/deleteadmin")]
    public async Task<IActionResult> DeleteAdmin(string email)
    {
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
}
