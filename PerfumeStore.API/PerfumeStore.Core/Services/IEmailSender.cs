using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
  public interface IEmailSender
  {
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
  }
}
