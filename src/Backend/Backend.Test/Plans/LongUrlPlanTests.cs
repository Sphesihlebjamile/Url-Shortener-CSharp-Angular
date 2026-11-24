namespace Backend.Test.Plans;

public class LongUrlPlanTests
{
    public readonly ILongUrlPlan _sut;
    public readonly IUrlShortenerDb _urlShortenerDb = Substitute.For<IUrlShortenerDb>();

    public LongUrlPlanTests()
    {
        _sut = new LongUrlPlan(_urlShortenerDb);
    }

    [Fact]
    public async Task LongUrlPlan_WhenValidInputThatExists_ShouldReturnLongUrl()
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
    [Fact]
    public async Task LongUrlPlan_WhenValidInputThatDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        string shortUrlCode = "G8sV09";
        Url? urlEntry = null;

        _urlShortenerDb.GetLongUrlByShortUrlCode(shortUrlCode, default)
            .Returns(urlEntry);

        // Act & Assert
        var result = await Should.ThrowAsync<LongUrlNotFoundException>(() => _sut.ExecuteAsync(shortUrlCode));
        var applicationException = result.ShouldBeAssignableTo<IApplicationException>();
        applicationException.ShouldNotBeNull();
        applicationException.Title.ShouldBe("Long Url Not Found");
        applicationException.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        applicationException.Errors.Count.ShouldBe(0);
        await _urlShortenerDb.DidNotReceive()
            .DisableShortUrl(Arg.Any<long>(), default);
    }

    [Fact]
    public async Task LongUrlPlan_WhenValidInputTHatExistsAndExpired_ShouldThrowNotFoundException()
    {
        string shortUrlCode = "G8sV09";
        Url urlEntry = new()
        {
            Id = Random.Shared.NextInt64(),
            CreatedDate = DateTimeOffset.UtcNow.AddMonths(-3),
            IsActive = true,
            ExpiryDate = DateTimeOffset.UtcNow.AddMonths(-2),
            LongUrl = "https://urlshortener.example",
            LastUpdated = DateTimeOffset.UtcNow.AddMonths(-3),
            ShortUrl = shortUrlCode
        };

        _urlShortenerDb.GetLongUrlByShortUrlCode(shortUrlCode, default)
            .Returns(urlEntry);

        // Act & Assert
        var result = await Should.ThrowAsync<LongUrlNotFoundException>(() => _sut.ExecuteAsync(shortUrlCode));
        await _urlShortenerDb.Received(1)
            .DisableShortUrl(urlEntry.Id, default);
        var applicationException = result.ShouldBeAssignableTo<IApplicationException>();
        applicationException.ShouldNotBeNull();
        applicationException.Title.ShouldBe("Long Url Not Found");
        applicationException.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        applicationException.Errors.Count.ShouldBe(0);
    }
}
