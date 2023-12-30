using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Domain.Repositories
{
    public interface ITokenService
    {
        public Task<string> GetToken(StoreUser user);
    }
}
