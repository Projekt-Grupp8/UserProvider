
﻿using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserProvider.Controllers;

[ApiController]
//[Authorize(Policy = "AuthenticatedAdmins")]
public class AdminCrudController : ControllerBase
{
    private readonly AdminCrudService _adminCrudService;

    public AdminCrudController(AdminCrudService adminCrudService)
    {
        _adminCrudService = adminCrudService;
    }

    [HttpPost]
    [Route("/createadmin")]
    public async Task<IActionResult> CreateAdmin(RegisterAdmin model)
    {
        // TODO Emma
        var createAdmin = await _adminCrudService.CreateAdminAsync(model);
        if (createAdmin is not null) 
        {
            return Ok(model.Email);
        }
        return BadRequest();
    }

    [HttpGet]
    [Route("/getadminbyid")]
    public async Task<IActionResult> GetOneAdmin(string email)
    {
        // TODO Emma
        var getAdmin = await _adminCrudService.GetOneAdminAsync(email);
        if (getAdmin is not null)
        {
			return Ok(email);
        }
        return NotFound($"{email} not found.");
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

        return NotFound("No admins found.");
    }

    [HttpPost]
    [Route("/updateadmin")]
    public async Task<IActionResult> UpdateAdmin(RegisterAdmin model)
    {
        // TODO Emma
        var updateAdmin = await _adminCrudService.UpdateAdminAsync(model);
        if (updateAdmin.ContentResult is not null)
        {
            return Ok(model.Email);
        }
        return NotFound(model.Email);
    }

    [HttpPost]
    [Route("/deleteadmin")]
    public async Task<IActionResult> DeleteAdmin()
    {
        // TODO Ted
        return new OkResult();
    }
}
