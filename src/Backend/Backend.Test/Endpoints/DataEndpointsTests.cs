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
        var request = new ApiDataRequest()
        {
            LongUrl = "https://www.example.com?jsndvljksblrgbsuidfvnkfdjgsd"
        };
        var urlShortenerOutput = new UrlShortenerOutput("https://short.url/abc123");
        var urlShortenerInput = new UrlShortenerInput(request.LongUrl);

        _orchestrator.ExecuteAsync(Arg.Any<UrlShortenerInput>()).Returns(urlShortenerOutput);
        _validator.Validate(Arg.Any<ApiDataRequest>()).Returns(new ValidationResult());


        // Act
        var response = await DataEndpoints.ShortenUrl(request, _validator, _orchestrator);


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
    public async Task ShortenUrl_WithInvalidInput_ShouldReturn400WithValidationErrorsWithoutCallingOrchestrator()
    {
        // Arrange
        var request = new ApiDataRequest()
        {
            LongUrl = "https://www.example.com?jsndvljksblrgbsuidfvnkfdjgsd"
        };
        var urlShortenerOutput = new UrlShortenerOutput("https://short.url/abc123");
        var urlShortenerInput = new UrlShortenerInput(request.LongUrl);

        _orchestrator.ExecuteAsync(Arg.Any<UrlShortenerInput>()).Returns(urlShortenerOutput);
        _validator.Validate(Arg.Any<ApiDataRequest>()).Returns(new ValidationResult([
            new ValidationFailure("LongUrl", "A long URL is required")
            ]));


        // Act
        var response = await DataEndpoints.ShortenUrl(request, _validator, _orchestrator);


        // Assert
        await _orchestrator.DidNotReceiveWithAnyArgs().ExecuteAsync(default!);
        var badResponse = response.ShouldBeOfType<BadRequest<List<ValidationFailure>>>();
        badResponse.ShouldNotBeNull();
        badResponse.StatusCode.ShouldBe(400);
        badResponse.Value.ShouldNotBeNull();
        badResponse.Value.Count.ShouldBe(1);
        badResponse.Value[0].PropertyName.ShouldBe("LongUrl");
        badResponse.Value[0].ErrorMessage.ShouldBe("A long URL is required");
    }
}
