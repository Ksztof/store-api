using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;

namespace Store.Application.Contracts.JwtToken;

public interface ITokenService
{
    public Task<Result<string>> IssueJwtToken(StoreUser user);
    public Task<Result<string>> IssueRefreshToken(StoreUser user);
    public Result RemoveAuthToken();
    public Result RemoveRefreshToken();
}
