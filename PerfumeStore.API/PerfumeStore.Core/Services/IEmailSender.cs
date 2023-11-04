using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
  public interface IEmailSender
  {
    public Task SendEmailAsync(string toEmail, string subject, string message);
    public Task Execute(string apiKey, string subject, string message, string toEmail);
  }
}
