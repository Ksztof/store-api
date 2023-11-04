using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PerfumeStore.Core.Services
{
  public interface IEmailService
  {
    public Task SendActivationLink(IdentityUser user);
    public Task ConfirmEmail(string userId, string encodedEmailToken); //TODO: exceptions are stupid
  }
}
