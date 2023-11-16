using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Core.Services
{
    public interface ITokenService
    {
        public Task<string> GetToken(StoreUser user);
    }
}
