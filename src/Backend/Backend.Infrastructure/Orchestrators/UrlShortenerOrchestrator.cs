namespace Backend.Infrastructure.Orchestrators;

public sealed class UrlShortenerOrchestrator :
    IUrlShortenerOrchestrator
{
    public readonly IUrlShortenerPlan _plan;

    public UrlShortenerOrchestrator(
        IUrlShortenerPlan plan)
    {
        _plan = plan;
    }
    public async Task<UrlShortenerOutput> ExecuteAsync(UrlShortenerInput request)
    {
        return await _plan.ExecuteAsync(request);
    }
}
