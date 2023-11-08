using DuendeIs.Core.DTOs.Request;
using DuendeIs.Core.DTOs.Response;

namespace DuendeIs.Core.Services
{
  public interface IUserService
  {
    public Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
    public Task RegisterUser(UserForRegistrationDto userForRegistration);
    public Task<bool> ConfirmEmail(string userId, string encodedEmailToken);
  }
}
