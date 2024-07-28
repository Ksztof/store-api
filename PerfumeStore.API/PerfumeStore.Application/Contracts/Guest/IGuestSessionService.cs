using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Contracts.Guest
{
    public interface IGuestSessionService
    {
        public UserResult SendCartIdToGuest(int cartId);

        public Result<int> GetCartId();

        public UserResult SetCartIdCookieAsExpired();
    }
}