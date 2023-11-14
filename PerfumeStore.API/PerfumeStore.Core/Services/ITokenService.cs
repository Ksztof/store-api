using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Services
{
    public interface ITokenService
    {
        public Task<string> GetToken(StoreUser user);
    }
}
