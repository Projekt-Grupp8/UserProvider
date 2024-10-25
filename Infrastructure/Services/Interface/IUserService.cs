using Infrastructure.Models;

namespace Infrastructure.Services.Interface
{
    public interface IUserService
    {
        Task<ResponseResult> CreateUserAsync(SignUpUser model);
        Task<string> GenerateTokenAsync(string email);
        Task<ResponseResult> SignInUserAsync(SignInUser user);
    }
}