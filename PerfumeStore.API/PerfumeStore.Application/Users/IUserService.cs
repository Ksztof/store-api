using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Application.Users
{
    public interface IUserService
    {
        public Task<AuthenticationResult> Login(AuthenticateUserDtoApp userForAuthentication);

        public Task<AuthenticationResult> RegisterUser(RegisterUserDtoApp userForRegistration);

        public Task<AuthenticationResult> ConfirmEmail(string userId, string emailToken);

        public Task<AuthenticationResult> RequestDeletion();

        public Task<AuthenticationResult> SubmitDeletion(string Id);

        public Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user);

        public Task<AuthenticationResult> FindByIdAsync(string userId);
    }
}
