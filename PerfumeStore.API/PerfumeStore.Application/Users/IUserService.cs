using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.Entities.StoreUsers;
using PerfumeStore.Domain.Shared.Abstractions;

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
