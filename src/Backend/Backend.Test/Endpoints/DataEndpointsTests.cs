namespace Backend.Test.Endpoints;

public class DataEndpointsTests
{
    private IValidator<ApiDataRequest> _validator;
    private IUrlShortenerOrchestrator _orchestrator;

    public DataEndpointsTests()
    {
        _validator = Substitute.For<IValidator<ApiDataRequest>>();
        _orchestrator = Substitute.For<IUrlShortenerOrchestrator>();
    }

    [Fact]
    public async Task ShortenUrl_WithValidInput_ShouldReturnAValidResponseWith200StatusCode()
    {
        // Arrange
        var httpContext = BuildHttpContext(true);
        var request = new ApiDataRequest()
        {
            LongUrl = "https://www.example.com?jsndvljksblrgbsuidfvnkfdjgsd"
        };
        var urlShortenerOutput = new UrlShortenerOutput("https://short.url/abc123");
        var urlShortenerInput = new UrlShortenerInput(request.LongUrl);

        _orchestrator.ExecuteAsync(Arg.Any<UrlShortenerInput>()).Returns(urlShortenerOutput);
        _validator.ValidateAsync(Arg.Any<ApiDataRequest>()).Returns(new ValidationResult());


        // Act
        var response = await DataEndpoints.ShortenUrl(request, _validator, httpContext, _orchestrator);


        // Assert
        await _orchestrator.Received(1)
            .ExecuteAsync(Arg.Is<UrlShortenerInput>(input => input.LongUrl == request.LongUrl));
        var okResponse = response.ShouldBeOfType<Created<ApiDataResponse>>();
        okResponse.ShouldNotBeNull();
        okResponse.Value.ShouldNotBeNull();
        okResponse.Value.ServerTime.ShouldBeOfType<long>();
        okResponse.Value.ShortUrl.ShouldBe(urlShortenerOutput.ShortUrl);
        okResponse.Location.ShouldNotBeNullOrWhiteSpace();
        okResponse.StatusCode.ShouldBe(201);
    }

    [Fact]
    public async Task ShortenUrl_WithInvalidInput_ShouldReturn422WithValidationErrorsWithoutCallingOrchestrator()
    {
        // Arrange
        var httpContext = BuildHttpContext(true);
        var request = new ApiDataRequest()
        {
            LongUrl = "https://www.example.com?jsndvljksblrgbsuidfvnkfdjgsd"
        };
        var urlShortenerOutput = new UrlShortenerOutput("https://short.url/abc123");
        var urlShortenerInput = new UrlShortenerInput(request.LongUrl);

        _orchestrator.ExecuteAsync(Arg.Any<UrlShortenerInput>()).Returns(urlShortenerOutput);
        _validator.ValidateAsync(Arg.Any<ApiDataRequest>()).Returns(new ValidationResult([
            new ValidationFailure("LongUrl", "A long URL is required")
            ]));


        // Act
        var response = await DataEndpoints.ShortenUrl(request, _validator, httpContext, _orchestrator);


        // Assert
        await _orchestrator.DidNotReceiveWithAnyArgs().ExecuteAsync(default!);
        var badResponse = response.ShouldBeOfType<JsonHttpResult<ApiErrorResponse>>();
        badResponse.ShouldNotBeNull();
        badResponse.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        var value = badResponse.Value;
        value.ShouldNotBeNull();
        value.Instance.ShouldBe("/api/data/shortenUrl");
        value.Title.ShouldBe("Validation Failure");
        value.Status.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        value.Errors.ShouldNotBeEmpty();
        value.Errors.Count.ShouldBe(1);
        value.Errors[0].ShouldBe("A long URL is required");
    }

    private static DefaultHttpContext BuildHttpContext(bool isDevelopment)
    {
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.Setup(e => e.EnvironmentName)
            .Returns(isDevelopment ? Environments.Development : Environments.Production);

        var services = new ServiceCollection();
        services.AddSingleton(hostEnvironment.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            Response =
            {
                Body = new MemoryStream()
            }
        };
        httpContext.Request.Path = "/api/data/shortenUrl";
        return httpContext;
    }
}
