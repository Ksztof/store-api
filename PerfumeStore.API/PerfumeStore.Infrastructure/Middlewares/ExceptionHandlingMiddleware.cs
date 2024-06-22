using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PerfumeStore.Application.Abstractions;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Infrastructure.Middlewares;
using System;
using System.Text.Json;
using System.Threading.Tasks;

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
            description: "Unexpected server error has occured, please contact application owner",
            errorType: ErrorType.Server
        );

        var problemDetailsResult = CreateProblemDetailsResult(error);
        var problemDetails = problemDetailsResult.Value as ProblemDetails;

        var json = JsonSerializer.Serialize(problemDetails);

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsync(json);
    }

    private static ObjectResult CreateProblemDetailsResult(Error error)
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
}
