using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Domain.Tokens
{
    public interface ITokenService
    {
        public Task<string> GetToken(StoreUser user);
    }
}
