
using Azure.Communication.Email;
using Azure;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService(Logger<EmailService> logger, IConfiguration configuration)
{
    private readonly ILogger<EmailService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task<bool> SendConfirmedRegistrationAsync(SignUpUser form)
    {
        var connectionString = _configuration.GetConnectionString("EmailService");
        var emailClient = new EmailClient(connectionString);

        var emailContent = new EmailContent("Tack för att du kontaktade oss!")
        {
            PlainText = $"Hej {form.UserName},\n\nTack för att du har registrerat dig på Rika! Vi är glada att ha dig med oss. " +
                        "Nu kan du börja utforska vårt breda utbud av produkter och dra nytta av våra exklusiva erbjudanden." +
                        "\n\nOm du har några frågor eller behöver hjälp, tveka inte att kontakta vårt supportteam. " +
                        "Vi finns här för att hjälpa dig.\n\nMed vänliga hälsningar,\nDitt team på Rika",

            Html = $"<p>Hej {form.UserName},</p>" +
                   "<p>Tack för att du har registrerat dig på <strong>Rika</strong>! Vi är glada att ha dig med oss. " +
                   "Nu kan du börja utforska vårt breda utbud av produkter och dra nytta av våra exklusiva erbjudanden.</p>" +
                   "<p>Om du har några frågor eller behöver hjälp, tveka inte att kontakta vårt supportteam. " +
                   "Vi finns här för att hjälpa dig.</p>" +
                   "<p>Med vänliga hälsningar,<br>Ditt team på Rika</p>"
        };

        var emailMessage = new EmailMessage(
            senderAddress: _configuration["EmailService:SenderAddress"],
            content: emailContent,
            recipients: new EmailRecipients(new List<EmailAddress>
            {
            new EmailAddress(form.Email)
            })
        );

        try
        {
            EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            return emailSendOperation.HasCompleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<SendConfirmedRegistrationAsync> E-mail confirmation failed");
            return false;
        }
    }
}
