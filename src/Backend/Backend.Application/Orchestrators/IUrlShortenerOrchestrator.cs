using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;

namespace Backend.Application.Orchestrators;

public interface IUrlShortenerOrchestrator : 
    IOrchestrator<UrlShortenerInput, UrlShortenerOutput>
{
}
