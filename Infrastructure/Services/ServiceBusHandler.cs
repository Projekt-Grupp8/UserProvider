using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class ServiceBusHandler
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceBusHandler> _logger;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly UserService _userService;

    public ServiceBusHandler(IConfiguration configuration, ILogger<ServiceBusHandler> logger, HttpClient httpClient, UserService userService)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        _userService = userService;
    }

    public async Task SendServiceBusMessageAsync(object body)
    {
        var connectionString = _configuration.GetConnectionString("ServiceBusConnectionString");
        var queue = "verification_request";

        try
        {
            await using var serviceBusClient = new ServiceBusClient(connectionString);
            var sender = serviceBusClient.CreateSender(queue);

            //var email = new { email =  body};
            var email = new { email = "ted.pieplow@gmail.com" };
            var json = JsonConvert.SerializeObject(email);
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
            string providerUri = _configuration.GetConnectionString("VerificationProviderString") ?? throw new ArgumentNullException("providerUri is null");

            var result = await _httpClient.PostAsJsonAsync(providerUri, new { Email = email, Code = code });
            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            await _userService.ChangeVerificationStatusAsync(email);
            return result.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError("SendServiceBusMessage() error : {Error}", ex.Message);
            return false;
        }
    }
}
