using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
  public interface IEmailService
  {
    Task SendActivationEmailAsync(IdentityUser user);
    public Task ConfirmEmail(string userId, string token);

  }
}
