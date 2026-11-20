namespace Backend.Api.Validation.DataEndpoints;

public class ApiDataRequestValidator : AbstractValidator<ApiDataRequest>
{
    public ApiDataRequestValidator(IHostEnvironment environment)
    {
        RuleFor(adr => adr.LongUrl)
            .NotNull()
            .NotEmpty()
                .WithMessage("A long URL is required")
            .MaximumLength(1000)
                .WithMessage("The long URL must not exceed 1000 characters")
            .MinimumLength(10)
                .WithMessage("The long URL must be at least 10 characters long")
            .Must((longUrl) => HttpUrlValidator.BeValidHttpUrl(longUrl, environment))
                .WithMessage("The long URL must be a valid HTTP or HTTPS URL with a valid domain")
            .When(adr => !string.IsNullOrWhiteSpace(adr.LongUrl) &&
                adr.LongUrl.Length is >= 10 and <= 1000,
                ApplyConditionTo.CurrentValidator);
    }
}
