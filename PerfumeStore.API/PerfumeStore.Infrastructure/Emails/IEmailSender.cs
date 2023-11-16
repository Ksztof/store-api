namespace PerfumeStore.Infrastructure.Emails
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message);
        public Task Execute(string apiKey, string subject, string message, string toEmail);
    }
}
