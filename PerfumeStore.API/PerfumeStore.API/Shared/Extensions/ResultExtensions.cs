using PerfumeStore.Application.Abstractions.Result.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.API.Shared.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return Results.Problem(
            statusCode: GetStatusCode(result.Error.Type),
            title: GetTitle(result.Error.Type),
            type: GetType(result.Error.Type),
            extensions: new Dictionary<string, object?>
            {
                    { "errors", new[] { result.Error} }
            });

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
}

