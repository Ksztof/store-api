using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PerfumeStore.Core.DTOs;

namespace PerfumeStore.Core.Services
{
  public class EmailSender : IEmailSender
  {
    private readonly SmtpClient _client;
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(SmtpSettings smtpSettings)
    {
      _smtpSettings = smtpSettings;
      _client = new SmtpClient
      {
        Host = _smtpSettings.Host,
        Port = _smtpSettings.Port,
        EnableSsl = _smtpSettings.EnableSsl,
        Credentials = new NetworkCredential(
          _smtpSettings.Username,
          _smtpSettings.Password
        )
      };
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
      try
      {
        var mailMessage = new MailMessage
        {
          From = new MailAddress(_smtpSettings.FromAddress),
          Subject = subject,
          Body = body,
          IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await _client.SendMailAsync(mailMessage);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Could not send email.", ex);
      }
    }
  }
}
