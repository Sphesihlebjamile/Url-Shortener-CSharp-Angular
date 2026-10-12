using Backend.Contracts.ApiResponseTypes;

namespace Backend.Api.Extensions;

public static class ProblemResultExtensions
{
    extension(ValidationResult result)
    {
        public IResult FromFluentValidationException(string instance, int statusCode)
        {
            var errors = result.Errors.Select(
                result => result.ErrorMessage)
                .Where(result => !string.IsNullOrWhiteSpace(result))
                .Distinct()
                .ToList();
            var apiErrorResponse = new ApiErrorResponse()
            {
                Errors = errors,
                Instance = instance,
                Status = statusCode,
                Title = "Validation Failure"
            };
            return Results.Json(apiErrorResponse, statusCode: statusCode);
        }
    }
}
