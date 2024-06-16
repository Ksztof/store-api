﻿using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Application.Users
{
    public interface IUserService
    {
        public Task<UserResult> Login(AuthenticateUserDtoApp userForAuthentication);

        public Task<UserResult> RegisterUser(RegisterUserDtoApp userForRegistration);

        public Task<UserResult> ConfirmEmail(string userId, string emailToken);

        public Task<UserResult> RequestDeletion();

        public Task<UserResult> SubmitDeletion(string Id);

        public Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user);

        public Task<UserResult> FindByIdAsync(string userId);
    }
}
