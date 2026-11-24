namespace Backend.Application.Persistence;

public interface IUrlShortenerDb
{
    Task<long> GetLatestUrlsIdAsync(CancellationToken cancellationToken);
    Task<Url?> GetShortUrlCodeByLongUrlAsync(string longUrl, CancellationToken cancellationToken);
    Task<bool> InsertNewUrlAsync(long id, string longUrl, string shortUrl, DateTimeOffset? expiryDate, CancellationToken cancellationToken);
    Task<Url?> GetLongUrlByShortUrlCode(string shortUrlCode, CancellationToken cancellationToken);
    Task<bool> EnableShortUrl(long id, DateTimeOffset? newExpiryDate, CancellationToken cancellationToken);
    Task<bool> DisableShortUrl(long id, CancellationToken cancellationToken);
}
