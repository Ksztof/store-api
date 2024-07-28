using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Shared;

namespace PerfumeStore.Application.Contracts.Guest
{
    public interface IGuestSessionService
    {
        public UserResult SendCartIdToGuest(int cartId);

        public Result<int> GetCartId();

        public UserResult SetCartIdCookieAsExpired();
    }
}