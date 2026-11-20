using Backend.Infrastructure.Extensions;

namespace Backend.Api.Validation;

public static class HttpUrlValidator
{
    private static readonly Regex DomainPattern = new(
        pattern: @"^(?=.{1,253}\.?$)[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?(?:\.[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?)+\.?$",
        options: RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Indicates whether the specified value is a valid absolute HTTP or HTTPS URL.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns><c>true</c> if the value is a valid absolute HTTP or HTTPS URL whose host matches the domain pattern; otherwise, <c>false</c>.</returns>
    public static bool BeValidHttpUrl(string? value, IHostEnvironment environemnt)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // This is the default behavior for local or development environments
        var isValid = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

        if (EnvironmentExtensions.IsQa(environemnt) || 
            EnvironmentExtensions.IsStaging(environemnt) ||
            EnvironmentExtensions.IsProduction(environemnt))
        {
            isValid = isValid && DomainPattern.IsMatch(uri.Host);
        }

        return isValid;
    }
}
