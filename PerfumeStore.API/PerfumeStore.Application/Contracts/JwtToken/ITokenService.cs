using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Contracts.JwtToken
{
    public interface ITokenService
    {
        public Task<Result> IssueJwtToken(StoreUser user);
        public Result RemoveAuthCookie();
    }
}
