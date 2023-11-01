using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfumeStore.Domain.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace PerfumeStore.Core.Services
{
  public class EmailService : IEmailSender
  {
    private readonly EmailConfiguration _emailConfig;
    public EmailService(IOptions<EmailConfiguration> emailConfig)
    {
      _emailConfig = emailConfig.Value;
    }
    public void SendEmail(Message message)
    {
      var emailMessage = CreateEmailMessage(message);

      Send(emailMessage);
    }

    public async Task SendEmailAsync(Message message)
    {
      var mailMessage = CreateEmailMessage(message);

      await SendAsync(mailMessage);
    }
    private MimeMessage CreateEmailMessage(Message message)
    {
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("API",_emailConfig.From));
      emailMessage.To.AddRange(message.To);
      emailMessage.Subject = message.Subject;

      var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

      if (message.Attachments != null && message.Attachments.Any())
      {
        byte[] fileBytes;
        foreach (var attachment in message.Attachments)
        {
          using (var ms = new MemoryStream())
          {
            attachment.CopyTo(ms);
            fileBytes = ms.ToArray();
          }

          bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
        }
      }

      emailMessage.Body = bodyBuilder.ToMessageBody();
      return emailMessage;
    }
    private void Send(MimeMessage mailMessage)
    {
      using (var client = new SmtpClient())
      {
        try
        {
          client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
          client.AuthenticationMechanisms.Remove("XOAUTH2");
          client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

          client.Send(mailMessage);
        }
        catch
        {
          //log an error message or throw an exception, or both.
          throw;
        }
        finally
        {
          client.Disconnect(true);
          client.Dispose();
        }
      }
    }

    private async Task SendAsync(MimeMessage mailMessage)
    {
      using (var client = new SmtpClient())
      {
        try
        {
          await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
          client.AuthenticationMechanisms.Remove("XOAUTH2");
          await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

          await client.SendAsync(mailMessage);
        }
        catch
        {
          //log an error message or throw an exception, or both.
          throw;
        }
        finally
        {
          await client.DisconnectAsync(true);
          client.Dispose();
        }
      }
    }
  }
}
