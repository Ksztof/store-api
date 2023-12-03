using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Users
{
    public interface IUserService
    {
        public Task<AuthenticationResult> Login(AuthenticateUserDtoApp userForAuthentication);
        public Task<RegistrationResponseDto> RegisterUser(RegisterUserDtoApp userForRegistration);
        public Task<bool> ConfirmEmail(string userId, string encodedEmailToken);
        public Task<bool> RequestDeletion();
        public Task<bool> SubmitDeletion(string id);
    }
}
