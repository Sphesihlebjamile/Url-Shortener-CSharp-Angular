namespace Backend.Api.Validation.DataEndpoints;

public class ApiDataRequestValidator : AbstractValidator<ApiDataRequest>
{
    private static readonly Regex DomainPattern = new(
        @"^(?=.{1,253}\.?$)[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?(?:\.[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?)+\.?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public ApiDataRequestValidator()
    {
        RuleFor(adr => adr.LongUrl)
            .NotNull()
            .NotEmpty()
                .WithMessage("A long URL is required")
            .MaximumLength(1000)
                .WithMessage("The long URL must not exceed 1000 characters")
            .MinimumLength(10)
                .WithMessage("The long URL must be at least 10 characters long")
            .Must(BeValidHttpUrl)
                .WithMessage("The long URL must be a valid HTTP or HTTPS URL with a valid domain")
            .When(adr => !string.IsNullOrWhiteSpace(adr.LongUrl) &&
                adr.LongUrl.Length is >= 10 and <= 1000,
                ApplyConditionTo.CurrentValidator);
    }

    /// <summary>
    /// Indicates whether the specified value is a valid absolute HTTP or HTTPS URL.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns><c>true</c> if the value is a valid absolute HTTP or HTTPS URL whose host matches the domain pattern; otherwise, <c>false</c>.</returns>
    private static bool BeValidHttpUrl(string? value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
            DomainPattern.IsMatch(uri.Host);
    }
}
