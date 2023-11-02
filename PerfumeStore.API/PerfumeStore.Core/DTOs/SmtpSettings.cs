using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.DTOs
{
  public class SmtpSettings
  {
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FromAddress { get; set; }
  }
}
