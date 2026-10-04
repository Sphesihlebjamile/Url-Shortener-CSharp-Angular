using Backend.Application.Capabilities;
using Backend.Application.Exceptions;
using Backend.Application.Options;
using Backend.Application.Orchestrators;
using Backend.Application.Persistence;
using Backend.Application.Plans;
using Backend.Contracts.ApiRequestTypes.DataEndpoint;
using Backend.Contracts.ApiResponseTypes.DataEndpoint;
using Backend.Infrastructure.Orchestrators;
using Backend.Infrastructure.Plans;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;

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
            ApplicationUrl = "https://localhost:7017"
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
        var urlShortenerInput = new UrlShortenerInput("sdgvsdfsdf");
        var expectedShortUrl = $"{_shortenerSettings.Value.ApplicationUrl}/{shortUrlCode}";
        var urlShortenerOutput = new UrlShortenerOutput(expectedShortUrl);

        _urlShortenerDb.GetShortUrlCodeByLongUrl(urlShortenerInput.LongUrl, CancellationToken.None)
            .Returns<string?>(string.Empty);
        _urlShortenerDb.GetLatestUrlsId(CancellationToken.None)
            .Returns(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns<string>(shortUrlCode);
        _urlShortenerDb.InsertNewUrl(uniqueId + 1, urlShortenerInput.LongUrl, shortUrlCode, CancellationToken.None)
            .Returns<bool>(true);

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

        _urlShortenerDb.GetShortUrlCodeByLongUrl(urlShortenerPlanInput.LongUrl, CancellationToken.None)
            .Returns<string?>(string.Empty);
        _urlShortenerDb.GetLatestUrlsId(CancellationToken.None)
            .Returns<long>(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns<string>(shortUrlCode);
        _urlShortenerDb.InsertNewUrl(uniqueId + 1, urlShortenerPlanInput.LongUrl, shortUrlCode, CancellationToken.None)
            .Returns<bool>(false);

        // Act & ASsert
        var result = await Should.ThrowAsync<ShortUrlGenerationException>(
            async () => await _sut.ExecuteAsync(urlShortenerPlanInput));
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Failed to insert new URL into database");
        result.Title.ShouldBe("Failed to generate short URL");
        result.StatusCode.ShouldBe(500);
        result.ShouldBeAssignableTo<IApplicationException>();
    }
}
