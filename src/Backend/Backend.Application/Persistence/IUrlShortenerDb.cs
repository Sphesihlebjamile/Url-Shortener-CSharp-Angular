namespace Backend.Application.Persistence;

public interface IUrlShortenerDb
{
    Task<long> GetLatestUrlsIdAsync(CancellationToken cancellationToken);
    Task<string?> GetShortUrlCodeByLongUrlAsync(string longUrl, CancellationToken cancellationToken);
    Task<bool> InsertNewUrlAsync(long id, string longUrl, string shortUrl, CancellationToken cancellationToken);
}
