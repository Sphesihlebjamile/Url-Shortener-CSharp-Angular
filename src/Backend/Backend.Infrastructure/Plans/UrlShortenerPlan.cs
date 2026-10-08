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

        // Validate that the longUrl does not exist in the database
        var shortUrlCode = await _urlShortenerDb.GetShortUrlCodeByLongUrl(request.LongUrl, CancellationToken.None);

        if(!string.IsNullOrWhiteSpace(shortUrlCode))
        {
            return new UrlShortenerOutput($"{_shortenerSettings.ApplicationUrl}/{shortUrlCode}");
        }

        // If it doesn't exist, get a globally unique Id
        var currentMaxId = await _urlShortenerDb.GetLatestUrlsId(CancellationToken.None);
        var newUniqueId = currentMaxId + 1;

        // Generate a base62 unique key
        var shortCode = _base62Converter.Execute(newUniqueId);

        // Save to database
        var dbResult = await _urlShortenerDb.InsertNewUrl(newUniqueId, request.LongUrl, shortCode, CancellationToken.None);

        if (!dbResult)
        {
            throw new ShortUrlGenerationException("Failed to insert new URL into database");
        }

        // Return
        var output = new UrlShortenerOutput($"{_shortenerSettings.ApplicationUrl}/{shortCode}");
        return output;
    }
}
