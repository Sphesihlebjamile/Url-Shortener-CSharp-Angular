namespace Backend.Test.Plans;

public class UrlShortenerPlanTests
{
    private readonly IUrlShortenerPlan _sut;
    private readonly IUrlShortenerDb _urlShortenerDb = Substitute.For<IUrlShortenerDb>();
    private readonly IBase62Converter _base62Converter = Substitute.For<IBase62Converter>();
    private readonly IOptions<ShortenerSettings> _shortenerSettings;

    public UrlShortenerPlanTests()
    {
        _shortenerSettings = Options.Create(new ShortenerSettings()
        {
            ApiApplicationUrl = "https://localhost:7017",
            ClientApplicationHomeUrl = "http://localhost:4200"
        });
        _sut = new UrlShortenerPlan(
            _urlShortenerDb,
            _base62Converter,
            _shortenerSettings);
    }

    [Fact]
    public async Task UrlShortenerPlan_WhenValidInput_ShouldReturnValidNewUrlShortenerOutput()
    {
        // Arrage
        var shortUrlCode = "4C92";
        var uniqueId = 1000000L;
        var urlShortenerPlanInput = new UrlShortenerInput("sdgvsdfsdf");
        var expectedShortUrl = $"{_shortenerSettings.Value.ApiApplicationUrl}/{shortUrlCode}";
        Url? getShortUrlByLongUrlOutput = null;

        _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns(getShortUrlByLongUrlOutput);
        _urlShortenerDb.GetLatestUrlsIdAsync(CancellationToken.None)
            .Returns(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns(shortUrlCode);
        _urlShortenerDb.InsertNewUrlAsync(uniqueId + 1, urlShortenerPlanInput.LongUrl, shortUrlCode, urlShortenerPlanInput.ExpiryDate, CancellationToken.None)
            .Returns(true);

        // Act
        var response = await _sut.ExecuteAsync(urlShortenerPlanInput);

        // Assert
        response.ShouldNotBeNull();
        response.ShortUrl.ShouldNotBeNullOrEmpty();
        response.ShortUrl.ShouldBe(expectedShortUrl);
    }

    [Fact]
    public async Task UrlShortenerPlan_WhenDatabaseFailed_ShouldThrowShortUrlGenerationException()
    {
        // Arrange
        var shortUrlCode = "4C92";
        var uniqueId = 1000000L;
        var urlShortenerPlanInput = new UrlShortenerInput("sdgvsdfsdf");
        Url? getShortUrlByLongUrlOutput = null;

        _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns(getShortUrlByLongUrlOutput);
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

    [Fact]
    public async Task UrlShortenerPlan_WhenLongUrlEmpty_ShouldThrowShortUrlGenerationException()
    {
        // Arrange
        var urlShortenerPlanInput = new UrlShortenerInput("");

        // Act & Assert
        var result = await Should.ThrowAsync<ShortUrlGenerationException>(
            async () => await _sut.ExecuteAsync(urlShortenerPlanInput));
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Long URL cannot be null or empty");
        result.Title.ShouldBe("Failed to generate short URL");
        result.StatusCode.ShouldBe(422);
        result.ShouldBeAssignableTo<IApplicationException>();
    }

    [Fact]
    public async Task UrlShortenerPlan_WhenLongUrlNull_ShouldThrowShortUrlGenerationException()
    {
        // Arrange
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var urlShortenerPlanInput = new UrlShortenerInput(null);
        #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act & Assert
        var result = await Should.ThrowAsync<ShortUrlGenerationException>(
            async () => await _sut.ExecuteAsync(urlShortenerPlanInput));
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Long URL cannot be null or empty");
        result.Title.ShouldBe("Failed to generate short URL");
        result.StatusCode.ShouldBe(422);
        result.ShouldBeAssignableTo<IApplicationException>();
    }

    [Fact]
    public async Task UrlShortenerPlan_WhenLongUrlAlreadyExists_ShouldReturnExistingShortUrl()
    {
        // Arrange
        var shortUrlCode = "4C92";
        var urlShortenerPlanInput = new UrlShortenerInput("sdgvsdfsdf");
        Url? getShortUrlByLongUrlOutput = new()
        {
            Id = Random.Shared.NextInt64(),
            CreatedDate = DateTimeOffset.UtcNow,
            ExpiryDate = null,
            IsActive = true,
            LastUpdated = DateTimeOffset.UtcNow,
            LongUrl = urlShortenerPlanInput.LongUrl,
            ShortUrl = shortUrlCode
        };

        _urlShortenerDb.GetShortUrlCodeByLongUrlAsync(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns(getShortUrlByLongUrlOutput);

        // Act
        var result = await _sut.ExecuteAsync(urlShortenerPlanInput);

        // Assert
        result.ShouldNotBeNull();
        result.ShortUrl.ShouldNotBeNullOrEmpty();
        result.ShortUrl.ShouldBe($"{_shortenerSettings.Value.ApiApplicationUrl}/{shortUrlCode}");
        await _urlShortenerDb.DidNotReceive().GetLatestUrlsIdAsync(Arg.Any<CancellationToken>());
        _base62Converter.DidNotReceive().Execute(Arg.Any<long>());
        await _urlShortenerDb.DidNotReceive().InsertNewUrlAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>());
    }
}
