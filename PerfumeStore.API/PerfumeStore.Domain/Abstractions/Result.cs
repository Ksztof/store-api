namespace PerfumeStore.Domain.Abstractions
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);

    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected internal Result(bool isSuccess, Error error, T? value)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, Error.None, value);
        public static Result<T> Failure(Error error) => new Result<T>(false, error, default);
    }
}
