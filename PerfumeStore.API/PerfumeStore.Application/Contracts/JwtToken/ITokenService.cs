using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Contracts.JwtToken
{
    public interface ITokenService
    {
        public Task<Result<string>> IssueJwtToken(StoreUser user);
        public Task<Result<string>> IssueRefreshToken(StoreUser user);
        public Result RemoveAuthToken();
        public Result RemoveRefreshToken();
    }
}
