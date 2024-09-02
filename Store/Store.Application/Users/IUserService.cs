﻿using Store.Application.Users.Dto.Request;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;

namespace Store.Application.Users
{
    public interface IUserService
    {
        public Task<UserResult> Login(AuthenticateUserDtoApp userForAuthentication);

        public Task<UserResult> RegisterUser(RegisterUserDtoApp userForRegistration);

        public Task<UserResult> ConfirmEmail(string userId, string token);

        public Task<UserResult> RequestDeletion();

        public Task<UserResult> SubmitDeletion(string userId);

        public Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user);

        public Task<UserResult> FindByIdAsync(string userId);

        public Result RemoveAuthToken();
        public Result RemoveRefreshToken();

        public UserResult RemoveGuestSessionId();
    }
}
