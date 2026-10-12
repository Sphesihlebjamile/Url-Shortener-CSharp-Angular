using Backend.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.Test.ApiInfrastructure;

public class GlobalExceptionHandlerTests
{

    [Fact]
    public async Task TryHandleAsync_WhenIApplicationException_ShouldReturnTrueWithProblemResult()
    {
        // Arrange
        var defaultHttpContext = BuildHttpContext(true);
        var hostEnvironment = GetHostEnvironment(true);
        var sut = new GlobalExceptionHandler(hostEnvironment);
        var exception = new ShortUrlGenerationException("Long URL cannot be null or empty", StatusCodes.Status422UnprocessableEntity);

        // Act
        var handled = await sut.TryHandleAsync(defaultHttpContext, exception, CancellationToken.None);

        // Assert
        handled.ShouldBe(true);
        exception.Message.ShouldBe("Long URL cannot be null or empty");
        exception.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        defaultHttpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        exception.ShouldBeAssignableTo<IApplicationException>();

        // Rewind the stream: Move the cursor back to the beginning
        defaultHttpContext.Response.Body.Position = 0;
        var body = await new StreamReader(defaultHttpContext.Response.Body).ReadToEndAsync();
        body.ShouldContain("Failed to generate short URL");
        body.ShouldContain("Long URL cannot be null or empty");
        body.ShouldContain("/api/data/shortenUrl");
    }

    [Fact]
    public async Task TryHandleAsync_WhenNotApplicationException_ShouldReturnTrueWithProblemResultInternalServerError()
    {
        // Arrange
        var defaultHttpContext = BuildHttpContext(true);
        var hostEnvironment = GetHostEnvironment(true);
        var sut = new GlobalExceptionHandler(hostEnvironment);
        var exception = new Exception("Just a test exception");

        // Act
        var handled = await sut.TryHandleAsync(defaultHttpContext, exception, CancellationToken.None);

        // Assert
        handled.ShouldBe(true);
        exception.Message.ShouldBe("Just a test exception");
        exception.ShouldNotBeAssignableTo<IApplicationException>();
        defaultHttpContext.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);

        defaultHttpContext.Response.Body.Position = 0;
        var body = await new StreamReader(defaultHttpContext.Response.Body).ReadToEndAsync();
        body.ShouldContain("An unexpected error has occurred!");
        body.ShouldContain("Just a test exception");
        body.ShouldContain("/api/data/shortenUrl");
    }

    [Fact]
    public async Task TryHandleAsync_WhenExceptionInProduction_ShouldReturnDefaultMessage()
    {
        // Arrange
        var defaultHttpContext = BuildHttpContext(false);
        var hostEnvironment = GetHostEnvironment(false);
        var sut = new GlobalExceptionHandler(hostEnvironment);
        var exception = new ShortUrlGenerationException("Long URL cannot be null or empty", StatusCodes.Status418ImATeapot);

        // Act
        var handled = await sut.TryHandleAsync(defaultHttpContext, exception, CancellationToken.None);
        defaultHttpContext.Response.StatusCode.ShouldBe(StatusCodes.Status418ImATeapot);

        // Assert
        handled.ShouldBe(true);

        defaultHttpContext.Response.Body.Position = 0;
        var body = await new StreamReader(defaultHttpContext.Response.Body).ReadToEndAsync();
        body.ShouldContain("An unexpected error occurred.");
    }

    private static IHostEnvironment GetHostEnvironment(bool isDevelopment)
    {
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.Setup(e => e.EnvironmentName)
            .Returns(isDevelopment ? Environments.Development : Environments.Production);
        return hostEnvironment.Object;
    }

    private static DefaultHttpContext BuildHttpContext(bool isDevelopment)
    {
        var environment = GetHostEnvironment(isDevelopment);

        var services = new ServiceCollection();
        services.AddSingleton(environment);

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
