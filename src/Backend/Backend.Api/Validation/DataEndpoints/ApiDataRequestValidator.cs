using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using FluentValidation;

namespace Backend.Api.Validation.DataEndpoints;

public class ApiDataRequestValidator : AbstractValidator<ApiDataRequest>
{
    public ApiDataRequestValidator()
    {
        RuleFor(adr => adr.LongUrl)
            .NotNull()
            .NotEmpty()
                .WithMessage("A long URL is required")
            .MaximumLength(1000)
                .WithMessage("The long URL must not exceed 1000 characters")
            .MinimumLength(10)
                .WithMessage("The long URL must be at least 10 characters long");
    }
}
