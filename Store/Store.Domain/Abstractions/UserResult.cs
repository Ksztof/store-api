using Store.Domain.StoreUsers;

namespace Store.Domain.Abstractions
{
    public class UserResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Token { get; }
        public Error Error { get; }
        public StoreUser? StoreUser { get; }

        public UserResult(bool isSuccess, Error error, StoreUser? storeUser = default, string? token = default)
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

        public static UserResult Success() => new(true, Error.None);
        public static UserResult Success(string token) => new(true, Error.None, null, token);
        public static UserResult Success(StoreUser storeUser) => new(true, Error.None, storeUser);
        public static UserResult Failure(Error error) => new(false, error, default);
    }
}
