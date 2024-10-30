﻿using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using ResponseStatusCode = Infrastructure.Models.StatusCode;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserProvider.Filters;
using Microsoft.AspNetCore.Authorization;

namespace UserProvider.Controllers
{
    [ApiController]
    public class AuthAdminController(ILogger<AuthUserController> logger, AdminService adminService, SignInManager<ApplicationUser> signInManager) : ControllerBase
    {
        private readonly ILogger<AuthUserController> _logger = logger;
        private readonly AdminService _adminService = adminService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;


        [HttpPost]
        [Route("/signinadmin")]
        public async Task<IActionResult> SignInAdmin(SignInAdmin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _adminService.SignIn(admin);

                return result.StatusCode switch
                {
                    ResponseStatusCode.OK => Ok(result),
                    ResponseStatusCode.NOT_FOUND => Conflict("Check your credentails, this user doesnt exist"),
                    ResponseStatusCode.UNAUTHORIZED => BadRequest("Invalid credentials"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected internal error occurred. Please try again later.")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "<Register> :: Registration failed due to an internal error: {StatusCode}", StatusCodes.Status500InternalServerError);
                return BadRequest("An unexpected internal error occurred. Please try again later.");
            }
        }
    }
}