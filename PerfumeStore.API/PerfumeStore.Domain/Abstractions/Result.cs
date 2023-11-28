using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Abstractions
{
    public class Result<TEntity>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public TEntity? Entity { get; }

        private Result(bool isSuccess, Error error, TEntity? entity = default)
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

        public static Result<TEntity> Success() => new(true, Error.None);
        public static Result<TEntity> Success(TEntity entity) => new(true, Error.None, entity);
        public static Result<TEntity> Failure(Error error) => new(false, error, default);
    }
}
