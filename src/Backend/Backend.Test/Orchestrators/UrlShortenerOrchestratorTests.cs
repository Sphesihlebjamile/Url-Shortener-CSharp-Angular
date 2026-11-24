namespace Backend.Test.Orchestrators;

public class UrlShortenerOrchestratorTests
{
    private readonly IUrlShortenerOrchestrator _sut;
    private readonly IUrlShortenerPlan _urlShortenerPlan;
    private readonly IUrlShortenerDb _urlShortenerDb = Substitute.For<IUrlShortenerDb>();
    private readonly IBase62Converter _base62Converter = Substitute.For<IBase62Converter>();
    private readonly IOptions<ShortenerSettings> _shortenerSettings;

    public UrlShortenerOrchestratorTests()
    {
        _shortenerSettings = Options.Create(new ShortenerSettings()
        {
            ApiApplicationUrl = "https://localhost:7017",
            ClientApplicationHomeUrl = "http://localhost:4200"
        });
        _urlShortenerPlan = new UrlShortenerPlan(
            _urlShortenerDb,
            _base62Converter,
            _shortenerSettings);
        _sut = new UrlShortenerOrchestrator(_urlShortenerPlan);
    }

    [Fact]
    public async Task UrlShortenerOrchestrator_WhenValidInput_ShouldReturnValidNewUrlShortenerOutput()
    {
        // Arrange
        var shortUrlCode = "4C92";
        var uniqueId = 1000000L;
        string longUrl = "https://urlshortener.example/home";
        var urlShortenerInput = new UrlShortenerInput(longUrl);
        var expectedShortUrl = $"{_shortenerSettings.Value.ApiApplicationUrl}/{shortUrlCode}";
        var urlShortenerOutput = new UrlShortenerOutput(expectedShortUrl);
        Url? getShortUrlByLongUrlOutput = null;

        _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(urlShortenerInput.LongUrl, CancellationToken.None)
            .Returns(getShortUrlByLongUrlOutput);
        _urlShortenerDb.GetLatestUrlsIdAsync(CancellationToken.None)
            .Returns(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns(shortUrlCode);
        _urlShortenerDb.InsertNewUrlAsync(uniqueId + 1, urlShortenerInput.LongUrl, shortUrlCode, urlShortenerInput.ExpiryDate, CancellationToken.None)
            .Returns(true);

        // Act

        var response = await _sut.ExecuteAsync(urlShortenerInput);

        // Assert
        response.ShouldNotBeNull();
        response.ShortUrl.ShouldNotBeNullOrEmpty();
        response.ShortUrl.ShouldBe(expectedShortUrl);
    }

    [Fact]
    public async Task UrlShortenerOrchestrator_WhenInvalidInput_ShouldThrowUrlShortenerGenerationException()
    {
        // Arrange
        var urlShortenerInput = new UrlShortenerInput("");

        // Act & Assert
        var exception = await Should.ThrowAsync<ShortUrlGenerationException>(
            async () => await _sut.ExecuteAsync(urlShortenerInput));

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("Long URL cannot be null or empty");
        exception.Title.ShouldBe("Failed to generate short URL");
        exception.StatusCode.ShouldBe(422);
        exception.ShouldBeAssignableTo<IApplicationException>();

    }

    [Fact]
    public async Task UrlShortenerOrchestrator_WhenDatabaseInsertFails_ShouldThrowUrlShortenerGenerationException()
    {
        // Arrange
        var shortUrlCode = "4C92";
        var uniqueId = 1000000L;
        var urlShortenerPlanInput = new UrlShortenerInput("sdgvsdfsdf");
        Url? getShortUrlCodeByLongUrlOutput = null;

        _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns(getShortUrlCodeByLongUrlOutput);
        _urlShortenerDb.GetLatestUrlsIdAsync(CancellationToken.None)
            .Returns(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns(shortUrlCode);
        _urlShortenerDb.InsertNewUrlAsync(uniqueId + 1, urlShortenerPlanInput.LongUrl, shortUrlCode, urlShortenerPlanInput.ExpiryDate, CancellationToken.None)
            .Returns(false);

        // Act & Assert
        var result = await Should.ThrowAsync<ShortUrlGenerationException>(
            async () => await _sut.ExecuteAsync(urlShortenerPlanInput));
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Failed to insert new URL into database");
        result.Title.ShouldBe("Failed to generate short URL");
        result.StatusCode.ShouldBe(500);
        result.ShouldBeAssignableTo<IApplicationException>();
    }
}
