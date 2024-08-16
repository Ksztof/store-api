﻿using Store.Domain.Shared;

namespace Store.Infrastructure.Middlewares
{
    internal sealed record GlobalExceptionError
    {
        public GlobalExceptionError(string code, string? description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            Type = errorType;
        }

        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }
    }
}