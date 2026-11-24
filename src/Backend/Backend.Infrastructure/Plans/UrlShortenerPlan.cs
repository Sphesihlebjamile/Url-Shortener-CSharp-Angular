namespace Backend.Infrastructure.Plans;

public sealed class UrlShortenerPlan :
    IUrlShortenerPlan
{
    private readonly IUrlShortenerDb _urlShortenerDb;
    private readonly IBase62Converter _base62Converter;
    private readonly ShortenerSettings _shortenerSettings;

    public UrlShortenerPlan(
        IUrlShortenerDb urlShortenerDb,
        IBase62Converter base62Converter,
        IOptions<ShortenerSettings> shortenerSettings)
    {
        _urlShortenerDb = urlShortenerDb;
        _base62Converter = base62Converter;
        _shortenerSettings = shortenerSettings.Value;
    }

    public async Task<UrlShortenerOutput> ExecuteAsync(UrlShortenerInput request)
    {
        if (string.IsNullOrWhiteSpace(request.LongUrl))
        {
            throw new ShortUrlGenerationException("Long URL cannot be null or empty", 422);
        }

        // Check for and return a database entry
        var urlEntry = await _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(request.LongUrl, CancellationToken.None);

        if(urlEntry != null)
        {
            await HandleUrlExpiry(urlEntry, request.ExpiryDate, default);

            return new UrlShortenerOutput($"{_shortenerSettings.ApiApplicationUrl}/{urlEntry.ShortUrl}");
        }

        // If it doesn't exist, get a globally unique Id
        var currentMaxId = await _urlShortenerDb.GetLatestUrlsIdAsync(CancellationToken.None);
        var newUniqueId = currentMaxId + 1;

        // Generate a base62 unique key
        var shortCode = _base62Converter.Execute(newUniqueId);

        // Save to database
        var dbResult = await _urlShortenerDb.InsertNewUrlAsync(newUniqueId, request.LongUrl, shortCode, request.ExpiryDate, CancellationToken.None);

        if (!dbResult)
        {
            throw new ShortUrlGenerationException("Failed to insert new URL into database");
        }

        // Return
        var output = new UrlShortenerOutput($"{_shortenerSettings.ApiApplicationUrl}/{shortCode}");
        return output;
    }

    /// <summary>
    /// This function manages expired short url codes that are attempted to be created again from a provided
    /// long url. If the short url code already exists and it is expired, then the function will mark
    /// the function as active and provide a new <c>Expiry Date</c>. If it is not expired then nothing will
    /// happen.
    /// </summary>
    /// <param name="urlEntry">The URL entry to inspect.</param>
    /// <param name="newExpiryDate">The new expiry date.</param>
    /// <returns></returns>
    private async Task HandleUrlExpiry(Url urlEntry, DateTimeOffset? newExpiryDate, CancellationToken cancellationToken)
    {
        if (urlEntry.ExpiryDate == null)
        {
            return;
        }

        // Url does exist, validate that is hasn't expired
        var hasExpired = urlEntry.ExpiryDate.Value.ToUniversalTime() <= DateTimeOffset.UtcNow || urlEntry.IsActive == false;

        if (hasExpired)
        {
            await _urlShortenerDb.EnableShortUrl(urlEntry.Id, newExpiryDate, cancellationToken);
        }
    }
}
