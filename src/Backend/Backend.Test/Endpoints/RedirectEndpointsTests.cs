using NSubstitute.ExceptionExtensions;

namespace Backend.Test.Endpoints;

public class RedirectEndpointsTests
{
    private ILongUrlOrchestrator _longUrlOrchestrator;
    private IOptions<ShortenerSettings> _shortenerSettingsOptions;

    public RedirectEndpointsTests()
    {
        _longUrlOrchestrator = Substitute.For<ILongUrlOrchestrator>();
        _shortenerSettingsOptions = Options.Create(new ShortenerSettings()
        {
            ApiApplicationUrl = "https://localhost:7017",
            ClientApplicationHomeUrl = "http://localhost:4200"
        });
    }

    [Fact]
    public async Task GetLongUrl_WithValidInput_ShouldReturnAPermanentHttpRedirect()
    {
        // Arrange
        var httpContext = BuildHttpContext(false);
        var shortUrlCode = "1x";
        var longUrlOutput = new LongUrlOutput("https://github.com/Sphesihlebjamile");

        _longUrlOrchestrator.ExecuteAsync(Arg.Any<string>()).Returns(longUrlOutput);

        // Act
        var response = await RedirectEndpoints.GetLongUrl(shortUrlCode, httpContext, _shortenerSettingsOptions, _longUrlOrchestrator);

        // Assert
        await _longUrlOrchestrator.Received(1)
            .ExecuteAsync(Arg.Is(shortUrlCode));

        var redirectResponse = response.ShouldBeOfType<RedirectHttpResult>();
        redirectResponse.ShouldNotBeNull();
        redirectResponse.Permanent.ShouldBeTrue();
        redirectResponse.Url.ShouldBe(longUrlOutput.LongUrl);
    }

    [Fact]
    public async Task GetLongUrl_WithInvalidInput_ShouldReturnATemporaryRedirectToClientHomeUrl()
    {
        // Arrange
        var httpContext = BuildHttpContext(false);
        var shortUrlCode = "";

        // Act
        var response = await RedirectEndpoints.GetLongUrl(shortUrlCode, httpContext, _shortenerSettingsOptions, _longUrlOrchestrator);

        // Assert
        await _longUrlOrchestrator.DidNotReceive()
            .ExecuteAsync(Arg.Any<string>());
        
        var redirectResponse = response.ShouldBeOfType<RedirectHttpResult>();
        redirectResponse.ShouldNotBeNull();
        redirectResponse.Permanent.ShouldBeFalse();
        redirectResponse.Url.ShouldBe(_shortenerSettingsOptions.Value.ClientApplicationHomeUrl);
    }

    [Fact]
    public async Task GetLongUrl_WithValidEndpointWithNoneExistingRecordInDb_ShouldReturnATemporaryRedirectToClientHomeUrl()
    {
        // Arrange
        var httpContext = BuildHttpContext(false);
        var shortUrlCode = "489vs";

        _longUrlOrchestrator.ExecuteAsync(Arg.Any<string>())
            .Throws(new LongUrlNotFoundException(
                $"{shortUrlCode}: A long url for the provided short url does not exist."));

        // Act
        var response = await RedirectEndpoints.GetLongUrl(shortUrlCode, httpContext, _shortenerSettingsOptions, _longUrlOrchestrator);

        // Assert
        await _longUrlOrchestrator.Received(1)
            .ExecuteAsync(Arg.Is(shortUrlCode));

        var redirectResponse = response.ShouldBeOfType<RedirectHttpResult>();
        redirectResponse.ShouldNotBeNull();
        redirectResponse.Permanent.ShouldBeFalse();
        redirectResponse.Url.ShouldBe(_shortenerSettingsOptions.Value.ClientApplicationHomeUrl);
    }

    private static DefaultHttpContext BuildHttpContext(bool isDevelopmnet)
    {
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.Setup(e => e.EnvironmentName)
            .Returns(isDevelopmnet ? Environments.Development : Environments.Production);

        var services = new ServiceCollection();
        services.AddSingleton(hostEnvironment.Object);

        var httpConext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            Response =
            {
                Body = new MemoryStream()
            }
        };
        httpConext.Request.Path = "/{shortUrlCode}";
        return httpConext;
    }
}
