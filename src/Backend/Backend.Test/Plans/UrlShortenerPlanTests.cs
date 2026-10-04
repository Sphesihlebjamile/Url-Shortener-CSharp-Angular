using Backend.Application.Capabilities;
using Backend.Application.Exceptions;
using Backend.Application.Options;
using Backend.Application.Persistence;
using Backend.Application.Plans;
using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Infrastructure.Plans;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;

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
            ApplicationUrl = "https://localhost:7017"
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
        var expectedShortUrl = $"{_shortenerSettings.Value.ApplicationUrl}/{shortUrlCode}";

        _urlShortenerDb.GetShortUrlCodeByLongUrl(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns<string?>(string.Empty);
        _urlShortenerDb.GetLatestUrlsId(CancellationToken.None)
            .Returns<long>(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns<string>(shortUrlCode);
        _urlShortenerDb.InsertNewUrl(uniqueId + 1, urlShortenerPlanInput.LongUrl, shortUrlCode, CancellationToken.None)
            .Returns<bool>(true);

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

        _urlShortenerDb.GetShortUrlCodeByLongUrl(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns<string?>(string.Empty);
        _urlShortenerDb.GetLatestUrlsId(CancellationToken.None)
            .Returns<long>(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns<string>(shortUrlCode);
        _urlShortenerDb.InsertNewUrl(uniqueId + 1, urlShortenerPlanInput.LongUrl, shortUrlCode, CancellationToken.None)
            .Returns<bool>(false);

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
        var urlShortenerPlanInput = new UrlShortenerInput(null);

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

        _urlShortenerDb.GetShortUrlCodeByLongUrl(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns<string?>(shortUrlCode);

        // Act
        var result = await _sut.ExecuteAsync(urlShortenerPlanInput);

        // Assert
        result.ShouldNotBeNull();
        result.ShortUrl.ShouldNotBeNullOrEmpty();
        result.ShortUrl.ShouldBe($"{_shortenerSettings.Value.ApplicationUrl}/{shortUrlCode}");
        await _urlShortenerDb.DidNotReceive().GetLatestUrlsId(Arg.Any<CancellationToken>());
        _base62Converter.DidNotReceive().Execute(Arg.Any<long>());
        await _urlShortenerDb.DidNotReceive().InsertNewUrl(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
