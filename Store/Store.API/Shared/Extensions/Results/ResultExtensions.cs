using Microsoft.AspNetCore.Mvc;
using Store.Domain.Abstractions;
using Store.Domain.Shared.Enums;

namespace Store.API.Shared.Extensions.Results;

public static class ResultExtensions
{
    public static IActionResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return CreateProblemDetailsResult(result.Error);
    }

    public static IActionResult ToProblemDetails(this UserResult result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return CreateProblemDetailsResult(result.Error);
    }

    public static IActionResult ToProblemDetails<TEntity>(this EntityResult<TEntity> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return CreateProblemDetailsResult(result.Error);
    }
    private static IActionResult CreateProblemDetailsResult(Error error)
    {
        var problemDetails = new ProblemDetails
        {
            Status = GetStatusCode(error.Type),
            Title = GetTitle(error.Type),
            Type = GetType(error.Type),
        };
        problemDetails.Extensions.Add("errors", new[] { error });

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Authorization => StatusCodes.Status403Forbidden,
            ErrorType.Authentication => StatusCodes.Status401Unauthorized,
            ErrorType.Server => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

    static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Authorization => "Forbidden",
            ErrorType.Authentication => "Unauthorized",
            ErrorType.Server => "Server Failure",
            _ => "Server Failure"
        };

    static string GetType(ErrorType statusCode) =>
        statusCode switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Authorization => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            ErrorType.Authentication => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorType.Server => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

}

