using Store.Domain.Shared.Enums;

namespace Store.Infrastructure.Middlewares;

internal sealed record GlobalExceptionError
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    public GlobalExceptionError(
        string code,
        string? description,
        ErrorType errorType)
    {
        Code = code;
        Description = description;
        Type = errorType;
    }
}
