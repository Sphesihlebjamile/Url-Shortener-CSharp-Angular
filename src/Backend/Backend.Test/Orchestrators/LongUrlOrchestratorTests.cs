namespace Backend.Test.Orchestrators;

public class LongUrlOrchestratorTests
{
    private readonly ILongUrlOrchestrator _sut;
    private readonly ILongUrlPlan _longUrlPlan;
    private readonly IUrlShortenerDb _urlShortenerDb = Substitute.For<IUrlShortenerDb>();

    public LongUrlOrchestratorTests()
    {
        _longUrlPlan = new LongUrlPlan(_urlShortenerDb);
        _sut = new LongUrlOrchestrator(_longUrlPlan);
    }

    [Fact]
    public async Task LongUrlOrchestrator_WhenValidInput_ShouldReturnValidLongUrlOutput()
    {
        // Arrange
        string shortUrlCode = "G8sV09";
        Url urlEntry = new()
        {
            Id = Random.Shared.NextInt64(),
            CreatedDate = DateTimeOffset.UtcNow.AddMonths(-3),
            IsActive = true,
            ExpiryDate = null,
            LongUrl = "https://urlshortener.example",
            LastUpdated = DateTimeOffset.UtcNow.AddMonths(-3),
            ShortUrl = shortUrlCode
        };

        _urlShortenerDb.GetLongUrlByShortUrlCode(shortUrlCode, default)
            .Returns(urlEntry);

        // Act
        var result = await _sut.ExecuteAsync(shortUrlCode);

        // Assert
        await _urlShortenerDb.DidNotReceive()
            .DisableShortUrl(Arg.Any<long>(), Arg.Any<CancellationToken>());
        result.LongUrl.ShouldBe(urlEntry.LongUrl);
    }
}
