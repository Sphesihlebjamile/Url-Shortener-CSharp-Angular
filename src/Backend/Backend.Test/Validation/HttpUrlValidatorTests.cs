namespace Backend.Test.Validation;

public class HttpUrlValidatorTests
{
    [Theory]
    [InlineData("https://order.example.com/?small=old-fashioned")]
    [InlineData("https://distribution.example.com/")]
    [InlineData("https://vague.example.com/groovy/stingy.php?morning=trade&unable=disgusted")]
    [InlineData("https://curl.example.com/complain.html")]
    [InlineData("https://volleyball.example.com/scold/houses")]
    public void BeValidHttpUrl_WhenValidHttpUrl_ShouldReturnTrue(string longUrl)
    {
        // Arrange
        IHostEnvironment _hostEnvironment = new FakeHostEnvironment(Environments.Production);

        // Act
        var response = HttpUrlValidator.BeValidHttpUrl(longUrl, _hostEnvironment);

        // Assert
        response.ShouldBe(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData("tps://order.example.com/?small=old-fashioned")]
    [InlineData("IAmJustARandomStringOfWords")]
    public void BeValidHttpUrl_WhenInvalidHttpUrl_ShouldReturnFalse(string longUrl)
    {
        // Arrange
        IHostEnvironment _hostEnvironment = new FakeHostEnvironment(Environments.Staging);

        // Act
        var response = HttpUrlValidator.BeValidHttpUrl(longUrl, _hostEnvironment);

        // Assert
        response.ShouldBe(false);
    }

    [Theory]
    [InlineData("https://localhost:7017")]
    [InlineData("http://localhost:5164")]
    [InlineData("https://localhost:7017/swagger")]
    public void BeValidHttpUrl_WhenHttpsOrHttpsAndNonPublicOrInvalidDomainOnLocalAndDevelopment_ShouldReturnTrue(string longUrl)
    {
        // Arrange
        IHostEnvironment _localHostEnvironment = new FakeHostEnvironment(EnvironmentExtensions.LocalEnvironmentName);
        IHostEnvironment _developmentHostEnvironment = new FakeHostEnvironment(Environments.Development);

        // Act
        var localResponse = HttpUrlValidator.BeValidHttpUrl(longUrl, _localHostEnvironment);
        var developmentResponse = HttpUrlValidator.BeValidHttpUrl(longUrl, _developmentHostEnvironment);

        // Assert
        localResponse.ShouldBe(true);
        developmentResponse.ShouldBe(true);
    }

    [Theory]
    [InlineData("https://localhost:7017")]
    [InlineData("http://localhost:5164")]
    [InlineData("https://localhost:7017/swagger")]
    public void BeValidHttpUrl_WhenHttpsOrHttpsAndNonPublicOrInvalidDomainInQaOrStagingOrProduction_ShouldReturnFalse(string longUrl)
    {
        // Arrange
        IHostEnvironment _qaHostEnvironment = new FakeHostEnvironment(EnvironmentExtensions.QAEnvironmentName);
        IHostEnvironment _stagingHostEnvironment = new FakeHostEnvironment(Environments.Staging);
        IHostEnvironment _productionHostEnvironment = new FakeHostEnvironment(Environments.Production);

        // Act
        var qaResponse = HttpUrlValidator.BeValidHttpUrl(longUrl, _qaHostEnvironment);
        var stagingResponse = HttpUrlValidator.BeValidHttpUrl(longUrl, _stagingHostEnvironment);
        var productionResponse = HttpUrlValidator.BeValidHttpUrl(longUrl, _productionHostEnvironment);

        // Assert
        qaResponse.ShouldBe(false);
        stagingResponse.ShouldBe(false);
        productionResponse.ShouldBe(false);
    }
}
