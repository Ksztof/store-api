using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
  public interface IUserService
  {
    public Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
    public Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
    public Task<bool> ConfirmEmail(string userId, string encodedEmailToken);
  }
}
