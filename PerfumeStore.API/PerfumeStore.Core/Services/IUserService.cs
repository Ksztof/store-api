using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;

namespace PerfumeStore.Core.Services
{
  public interface IUserService
  {
    public Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
    public Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
    public Task<bool> ConfirmEmail(string userId, string encodedEmailToken);
  }
}
