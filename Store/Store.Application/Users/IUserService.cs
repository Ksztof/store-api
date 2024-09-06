using Store.Application.Users.Dto.Request;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;

namespace Store.Application.Users;

public interface IUserService
{
    public Task<UserResult> LoginAsync(AuthenticateUserDtoApp userForAuthentication);
    public Task<UserResult> RegisterUserAsync(RegisterUserDtoApp userForRegistration);
    public Task<UserResult> ConfirmEmailAsync(string userId, string token);
    public Task<UserResult> RequestDeletionAsync();
    public Task<UserResult> SubmitDeletionAsync(string userId);
    public Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user);
    public Task<UserResult> FindByIdAsync(string userId);
    public Result RemoveAuthToken();
    public Result RemoveRefreshToken();
    public UserResult RemoveGuestSessionId();
}
