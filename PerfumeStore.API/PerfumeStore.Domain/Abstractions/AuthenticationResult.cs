using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.StoreUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Abstractions
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Token { get; }
        public Error Error { get; }
        public StoreUser? StoreUser { get; }

        public AuthenticationResult(bool isSuccess, Error error, StoreUser? storeUser = default, string? token = default)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
            StoreUser = storeUser;
            Token = token;
        }

        public static AuthenticationResult Success() => new(true, Error.None);
        public static AuthenticationResult Success(string token) => new(true, Error.None, null, token);
        public static AuthenticationResult Success(StoreUser storeUser) => new(true, Error.None, storeUser);
        public static AuthenticationResult Failure(Error error) => new(false, error, default);
    }
}
