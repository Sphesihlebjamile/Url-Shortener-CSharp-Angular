namespace Backend.Application.Persistence;

public interface IUrlShortenerDb
{
    Task<long> GetLatestUrlsId(CancellationToken cancellationToken);
}
