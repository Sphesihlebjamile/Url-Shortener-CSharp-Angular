using Backend.Application.Plans;
using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;

namespace Backend.Infrastructure.Plans;

public sealed class UrlShortenerPlan :
    IUrlShortenerPlan
{
    public Task<UrlShortenerOutput> ExecuteAsync(UrlShortenerInput request)
    {
        var output = new UrlShortenerOutput("https://localhost/7017");
        return Task.FromResult(output);
    }
}
