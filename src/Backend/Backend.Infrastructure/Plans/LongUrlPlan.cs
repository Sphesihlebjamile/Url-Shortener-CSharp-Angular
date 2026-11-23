using Backend.Domain.Entities;

namespace Backend.Infrastructure.Plans;

public sealed class LongUrlPlan :
    ILongUrlPlan
{
    public readonly IUrlShortenerDb _urlShortenerDb;

    public LongUrlPlan(
        IUrlShortenerDb urlShortenerDb)
    {
        _urlShortenerDb = urlShortenerDb;
    }

    public async Task<LongUrlOutput> ExecuteAsync(string shortUrlCode)
    {
        // Get the long Url from the database using the short URL
        var urlEntry = await _urlShortenerDb.GetLongUrlByShortUrlCode(shortUrlCode, default);

        if (urlEntry == null)
        {
            throw new LongUrlNotFoundException($"{shortUrlCode}: A long url for the provided short url does not exist.");
        }

        if(urlEntry.ExpiryDate != null &&
            urlEntry.ExpiryDate.Value.ToUniversalTime() <= DateTimeOffset.UtcNow)
        {
            await _urlShortenerDb.DisableShortUrl(urlEntry.Id, default);

            throw new LongUrlNotFoundException($"{shortUrlCode}: A long url for the provided short url has expired.");
        }

        return new LongUrlOutput(urlEntry.LongUrl);
    }
}
