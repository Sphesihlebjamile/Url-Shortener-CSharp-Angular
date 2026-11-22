namespace Backend.Api.Endpoints;

public static class RedirectEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapRedirectEndpoints(
        this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/{shortUrlCode}", GetLongUrl)
            .WithName("GetLongUrl")
            .WithDisplayName("Get Long URL Endpoint")
            .WithDescription("Accepts a shortened URL as input and returns the original long URL")
            .AllowAnonymous()
            .Produces<ApiLongUrlResponse>(StatusCodes.Status200OK, contentType: "application/json")
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return builder;
    }

    public static async Task<IResult> GetLongUrl(
        [FromRoute] string shortUrlCode,
        HttpContext httpContext,
        IOptions<ShortenerSettings> shortenerSettings,
        ILongUrlOrchestrator orchestrator)
    {
        var validationResult = LongUrlRequestValidator.ValidateShortUrlCode(shortUrlCode);
        if (!validationResult.IsValid)
        {
            return Results.Redirect(shortenerSettings.Value.ClientApplicationHomeUrl, permanent: false);
        }

        try
        {
            var results = await orchestrator.ExecuteAsync(shortUrlCode);
            return Results.Redirect(results.LongUrl, permanent: true);
        }
        catch (Exception ex)
        {
            return Results.Redirect(shortenerSettings.Value.ClientApplicationHomeUrl, permanent: false);
        }
    }
}
