using Backend.Application.Plans;
using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;

namespace Backend.Infrastructure.Plans;

public sealed class UrlShortenerPlan :
    IUrlShortenerPlan
{
    public Task<UrlShortenerOutput> ExecuteAsync(UrlShortenerInput request)
    {
        // Validate that the longUrl does not exist in the database

        // If it doesn't exist, get a globally unique Id

        // Generate a base62 unique key

        // Save to database

        // Return
        var output = new UrlShortenerOutput("https://localhost/7017");
        return Task.FromResult(output);
    }
}
