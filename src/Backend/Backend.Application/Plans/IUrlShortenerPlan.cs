using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;

namespace Backend.Application.Plans;

public interface IUrlShortenerPlan :
    IPlan<UrlShortenerInput, UrlShortenerOutput>
{
}
