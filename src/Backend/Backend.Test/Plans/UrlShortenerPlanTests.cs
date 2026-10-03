using Backend.Application.Capabilities;
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
        var d = new UrlShortenerInput("sdgvsdfsdf");

        _urlShortenerDb.GetShortUrlCodeByLongUrl(d.LongUrl, CancellationToken.None)
            .Returns<string?>(string.Empty);
        _urlShortenerDb.GetLatestUrlsId(CancellationToken.None)
            .Returns<long>(uniqueId);
        _base62Converter.Execute(uniqueId + 1)
            .Returns<string>(shortUrlCode);
        _urlShortenerDb.InsertNewUrl(uniqueId + 1, d.LongUrl, shortUrlCode, CancellationToken.None)
            .Returns<bool>(true);

        // Act
        var response = await _sut.ExecuteAsync(d);

        // Assert
        response.ShouldNotBeNull();
        response.ShortUrl.ShouldNotBeNullOrEmpty();
    }
}
