using Backend.Contracts.ApiResponseTypes.DataEndpoint;

namespace Backend.Api.Endpoints;

public static class DataEndpoints
{
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
            .Produces<ApiDataResponse>(StatusCodes.Status201Created, contentType: "application/json");

        return builder;
    }

    public static IResult ShortenUrl()
    {
        var response = new ApiDataResponse()
        {
            ShortUrl = "Short Url"
        };
        return Results.Created("/data/shorten", response);
    }
}
