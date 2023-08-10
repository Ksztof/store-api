using IdentityModel.Client;

namespace PerfumeShop.Client.Services
{
  public interface ITokenService
  {
        Task<TokenResponse> GetToken(string scope);
  }
}
 