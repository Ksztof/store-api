using Store.Domain.Abstractions;

namespace Store.Application.Contracts.Guest;

public interface IGuestSessionService
{
    public UserResult SendCartIdToGuest(int cartId);
    public Result<int> GetCartId();
    public UserResult SetCartIdCookieAsExpired();
}