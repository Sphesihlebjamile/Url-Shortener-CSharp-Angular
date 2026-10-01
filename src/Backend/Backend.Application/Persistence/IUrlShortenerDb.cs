namespace Backend.Application.Persistence;

public interface IUrlShortenerDb
{
    Task<long> GetLatestUrlsId(CancellationToken cancellationToken);
    Task<string?> GetShortUrlCodeByLongUrl(string longUrl, CancellationToken cancellationToken);
    Task<bool> InsertNewUrl(long id, string longUrl, string shortUrl, CancellationToken cancellationToken);
}
