using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AdminCrudService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminCrudService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResponseResult> CreateAdminAsync(RegisterAdmin model)
    {
        // TODO
        return null!;
    }

    public async Task<ResponseResult> GetAdminByIdAsync(string email)
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> GetAllAdmin()
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> UpdateAdminAsync(RegisterAdmin model)
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> DeleteAdminAsync(string email)
    {
        // TODO

        return null!;
    }

    public async Task<ResponseResult> ExistsAsync(string email)
    {
        // TODO

        return null!;
    }
}
