using IdentityModel.Client;

namespace PerfumeStore.Core.Services
{
    public interface ITokenService
    {
        public Task<TokenResponse> GetToken(string scope);
    }
}
