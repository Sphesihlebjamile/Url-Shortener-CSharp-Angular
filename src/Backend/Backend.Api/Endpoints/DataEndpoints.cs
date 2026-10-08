using Backend.Application.Orchestrators;
using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
            .Produces<ApiDataResponse>(StatusCodes.Status201Created, contentType: "application/json");

        return builder;
    }

    public static async Task<IResult> ShortenUrl(
        [FromBody] ApiDataRequest request,
        IValidator<ApiDataRequest> validator,
        IUrlShortenerOrchestrator orchestrator)
    {
        var timer = Stopwatch.StartNew();
        // Validate request object
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        // call orchestrator to execute on the url shortening
        var input = new UrlShortenerInput(request.LongUrl!);
        var orchResult = await orchestrator.ExecuteAsync(input);

        // map output to response object
        var response = new ApiDataResponse()
        {
            ShortUrl = orchResult.ShortUrl,
        };

        timer.Stop();
        
        response.ServerTime = timer.ElapsedMilliseconds;

        // return response
        return Results.Created("/data/shorten", response);
    }
}
