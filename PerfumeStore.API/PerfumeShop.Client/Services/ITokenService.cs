using IdentityModel.Client;

namespace PerfumeShop.Client.Services
{
    public interface ITokenService
    {
        public Task<TokenResponse> GetToken(string scope);
    }
}
