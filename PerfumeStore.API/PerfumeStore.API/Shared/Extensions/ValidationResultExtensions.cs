using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Application.Abstractions;
using PerfumeStore.Application.Abstractions.Result.Shared;
using System.ComponentModel.DataAnnotations;
using FluentValidationResults = FluentValidation.Results.ValidationResult;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using PerfumeStore.Application.Abstractions.Result;

namespace PerfumeStore.API.Shared.Extensions
{
    public static class ValidationResultExtensions
    {
        public static IActionResult ToValidationProblemDetails(this FluentValidationResults results)
        {
            if (results.IsValid)
            {
                throw new InvalidOperationException();
            }

            return CreateValidationProblemDetailsResult(results.Errors);
        }

        private static IActionResult CreateValidationProblemDetailsResult(IEnumerable<ValidationFailure> fluentValidationErrors)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            };

            IEnumerable<Error> validationErrors = fluentValidationErrors.Select(error =>
            ValidationErrors.FormValidationError(error.PropertyName, error.ErrorMessage)).ToArray();

            problemDetails.Extensions.Add("errors", validationErrors);

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }
    }
}
