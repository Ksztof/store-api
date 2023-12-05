using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Users
{
    public interface IUserService
    {
        public Task<AuthenticationResult> Login(AuthenticateUserDtoApp userForAuthentication);
        public Task<AuthenticationResult> RegisterUser(RegisterUserDtoApp userForRegistration);
        public Task<bool> ConfirmEmail(string userId, string encodedEmailToken);
        public Task<AuthenticationResult> RequestDeletion();
        public Task<AuthenticationResult> SubmitDeletion(string Id);
    }
}
