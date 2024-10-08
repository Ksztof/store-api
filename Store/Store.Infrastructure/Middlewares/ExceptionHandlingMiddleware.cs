using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Domain.Shared.Enums;
using System.Text.Json;

namespace Store.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        GlobalExceptionError error = new GlobalExceptionError(
            code: "UnexpectedError.Status500InternalServerError",
            description: $"Unexpected server error has occurred with message: {exception.Message}. please contact application owner",
            errorType: ErrorType.Server
        );

        ObjectResult problemDetailsResult = CreateProblemDetailsResult(error);
        ProblemDetails? problemDetails = problemDetailsResult.Value as ProblemDetails;

        if (problemDetails == null)
        {
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Conversion Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            };

            GlobalExceptionError conversionError = new GlobalExceptionError(
                        code: "ProblemDetails.ConversionError",
                        description: "Unexpected server error has occurred, durring ObjectResult conversion",
                        errorType: ErrorType.Server
                    );
            problemDetails.Extensions.Add("errors", new[] { conversionError });

        }

        string json = JsonSerializer.Serialize(problemDetails);

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsync(json);
    }

    private static ObjectResult CreateProblemDetailsResult(GlobalExceptionError error)
    {
        ProblemDetails problemDetails = new ProblemDetails
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
