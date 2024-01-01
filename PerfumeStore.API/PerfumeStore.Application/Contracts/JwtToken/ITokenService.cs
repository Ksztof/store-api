using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Application.Contracts.JwtToken
{
    public interface ITokenService
    {
        public Task<string> GetToken(StoreUser user);
    }
}
