using PerfumeStore.Application.Abstractions.Result.Shared;

namespace PerfumeStore.Application.Abstractions.Result.Entity
{
    public class EntityResult<TEntity>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public TEntity? Entity { get; }

        public EntityResult(bool isSuccess, Error error, TEntity? entity = default)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
            Entity = entity;
        }

        public static EntityResult<TEntity> Success() => new(true, Error.None);
        public static EntityResult<TEntity> Success(TEntity entity) => new(true, Error.None, entity);
        public static EntityResult<TEntity> Failure(Error error) => new(false, error, default);
    }
}
