using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.Email;

namespace Store.Infrastructure.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly KeyVaultOptions _keyVaultOptions;

        public EmailSender(IOptions<KeyVaultOptions> keyVaultOptions,
          ILogger<EmailSender> logger)
        {
            _keyVaultOptions = keyVaultOptions.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_keyVaultOptions.SendgridKey))
            {
                throw new Exception("Null SendGridKey");
            }

            await Execute(_keyVaultOptions.SendgridKey, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("krzysztofkozlowski1995@gmail.com", "Store"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
              ? $"Email to {toEmail} queued successfully!"
              : $"Failure Email to {toEmail}");
        }
    }
}
