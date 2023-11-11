using IdentityModel.Client;
using PerfumeStore.Core.DTOs.Request;

namespace PerfumeStore.Core.Services
{
  public interface ITokenService
  {
    public Task<TokenResponse> GetToken(string scope, UserForAuthenticationDto userForAuthentication);
  }
}