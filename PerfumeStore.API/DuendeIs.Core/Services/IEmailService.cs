using DuendeIs.Core.Models;

namespace DuendeIs.Core.Services
{
  public interface IEmailService
  {
    public Task SendActivationLink(StoreUser user);
    public Task ConfirmEmail(string userId, string encodedEmailToken); //TODO: exceptions are stupid
  }
}
