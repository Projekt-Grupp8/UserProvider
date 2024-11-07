using Infrastructure.Models;

namespace Infrastructure.Services.Interface
{
    public interface IServiceBusHandler
    {
        Task<ResponseResult> ChangeVerificationStatusAsync(string email);
        Task SendServiceBusMessageAsync(string email);
        Task SendWelcomeMessageAsync(string email);
        Task<bool> VerifyCodeAsync(string email, string code);
    }
}