namespace Backend.Api.Endpoints;

public static class DataEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapDataEndpoints(
        this IEndpointRouteBuilder builder)

    {
        // Group endpoints under the "/data" route/prefix
        var group = builder.MapGroup("api/v1/data");

        group.MapPost("/shorten", ShortenUrl)
            .WithName("ShortenUrl")
            .WithDisplayName("Shorten URL Endpoint")
            .WithDescription("Accepts a long URL as input and returns a shortened version")
            .AllowAnonymous()
            .Produces<ApiDataResponse>(StatusCodes.Status201Created, contentType: "application/json")
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/long/{shortUrlCode}", GetLongUrl)
            .WithName("GetLongUrl")
            .WithDisplayName("Get Long URL Endpoint")
            .WithDescription("Accepts a shortened URL as input and returns the original long URL")
            .AllowAnonymous()
            .Produces<ApiLongUrlResponse>(StatusCodes.Status200OK, contentType: "application/json")
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return builder;
    }

    public static async Task<IResult> ShortenUrl(
        [FromBody] ApiDataRequest request,
        IValidator<ApiDataRequest> validator,
        HttpContext httpContext,
        IUrlShortenerOrchestrator orchestrator)
    {
        var timer = Stopwatch.StartNew();

        // Validate request object
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.FromFluentValidationException(httpContext.Request.Path, StatusCodes.Status422UnprocessableEntity);
        }

        // call orchestrator to execute on the url shortening
        var input = new UrlShortenerInput(request.LongUrl!);
        var orchResult = await orchestrator.ExecuteAsync(input);

        // map output to response object
        var response = new ApiDataResponse()
        {
            ShortUrl = orchResult.ShortUrl,
        };

        // Collect execution metrics
        timer.Stop();
        response.ServerTime = timer.ElapsedMilliseconds;

        // return response
        return Results.Created("/data/shorten", response);
    }

    public static async Task<IResult> GetLongUrl(
        [FromRoute] string shortUrlCode,
        HttpContext httpContext,
        ILongUrlOrchestrator orchestrator)
    {
        var timer = Stopwatch.StartNew();

        var validationResult = LongUrlRequestValidator.ValidateShortUrlCode(shortUrlCode);
        if (!validationResult.IsValid)
        {
            return validationResult.FromFluentValidationException(httpContext.Request.Path, StatusCodes.Status422UnprocessableEntity);
        }

        var results = await orchestrator.ExecuteAsync(shortUrlCode);

        timer.Stop();

        var response = new ApiLongUrlResponse()
        {
            LongUrl = results.LongUrl,
            ServerTime = timer.ElapsedMilliseconds
        };

        return Results.Ok(response);
    }
}
