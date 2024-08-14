using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Domain.Shared;
using Store.Infrastructure.Middlewares;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new GlobalExceptionError(
            code: "UnexpectedError.Status500InternalServerError",
            description: "Unexpected server error has occurred, please contact application owner",
            errorType: ErrorType.Server
        );

        var problemDetailsResult = CreateProblemDetailsResult(error);
        var problemDetails = problemDetailsResult.Value as ProblemDetails;

        if (problemDetails == null)
        {
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Conversion Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            };

            var conversionError = new GlobalExceptionError(
                        code: "ProblemDetails.ConversionError",
                        description: "Unexpected server error has occurred, durring ObjectResult conversion",
                        errorType: ErrorType.Server
                    );
            problemDetails.Extensions.Add("errors", new[] { conversionError });

        }

        var json = JsonSerializer.Serialize(problemDetails);

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsync(json);
    }

    private static ObjectResult CreateProblemDetailsResult(GlobalExceptionError error)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Critical Server Failure",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        };
        problemDetails.Extensions.Add("errors", new[] { error });

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}
