using Azure.Core.Pipeline;
using Azure.Messaging.ServiceBus;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class ServiceBusHandler : IServiceBusHandler
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceBusHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly UserManager<ApplicationUser> _userManager;


    public ServiceBusHandler(IConfiguration configuration, ILogger<ServiceBusHandler> logger, HttpClient httpClient, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        _userManager = userManager;
    }

    public async Task SendServiceBusMessageAsync(string email)
    {
        // Flytta över till appsettings.
        var connectionString = _configuration.GetConnectionString("ServiceBusConnectionString");
        var queue = "verification_request";

        try
        {
            await using var serviceBusClient = new ServiceBusClient(connectionString);
            var sender = serviceBusClient.CreateSender(queue);

            var body = new { Email = email };
            var json = JsonConvert.SerializeObject(body);
            var message = new ServiceBusMessage(json);
            await sender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError("SendServiceBusMessage() error : {Error}", ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }

        return;
    }

    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        try
        {
            string providerUri = _configuration.GetConnectionString("VerificationProviderString")
                                ?? throw new ArgumentNullException(nameof(providerUri));

            var result = await _httpClient.PostAsJsonAsync(providerUri, new { Email = email, Code = code });
            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            await ChangeVerificationStatusAsync(email);
            await SendWelcomeMessageAsync(email);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("VerifyCodeAsync() error : {Error}", ex.Message);
            return false;
        }
    }

    public async Task<ResponseResult> ChangeVerificationStatusAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ResponseFactory.NotFound("The user doesnt exist, please try again.");
            }

            if (!user.IsVerified)
            {
                user!.IsVerified = true;
                await _userManager.UpdateAsync(user);
            }

            return ResponseFactory.Ok("Verification status updated", user!.IsVerified);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<ChangeVerificationStatusAsync> Failed changing verification status.");
            return ResponseFactory.InternalError();
        }
    }

    public async Task SendWelcomeMessageAsync(string email)
    {
        // Flytta över till appsettings.
        var connectionString = _configuration.GetConnectionString("ServiceBusConnectionString");
        var queue = "welcome_request";

        try
        {
            await using var serviceBusClient = new ServiceBusClient(connectionString);
            var sender = serviceBusClient.CreateSender(queue);

            var body = new { Email = email };
            var json = JsonConvert.SerializeObject(body);
            var message = new ServiceBusMessage(json);
            await sender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError("SendServiceBusMessage() error : {Error}", ex.Message);
        }
    }
}
