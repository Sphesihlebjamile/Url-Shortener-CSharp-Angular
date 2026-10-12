namespace Backend.Test.Validation.DataEndpoints;

public class ApiDataRequestValidatorTests
{
    private readonly IValidator<ApiDataRequest> _validator = new ApiDataRequestValidator();
    [Fact]
    public async Task ValidateAsync_WhenNoLongUrlProvided_ShouldFailWithErrors()
    {
        // Arrange
        var request = new ApiDataRequest();

        // Act
        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ElementAt(0).ErrorMessage.ShouldBe("'Long Url' must not be empty.");
        result.Errors.ElementAt(1).ErrorMessage.ShouldBe("A long URL is required");
    }

    [Fact]
    public async Task ValidateAsync_WhenLongUrlTooShort_ShouldFailWithError()
    {
        // Arrange
        var request = new ApiDataRequest()
        {
            LongUrl = "Short"
        };

        // Act
        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ElementAt(0).ErrorMessage.ShouldBe("The long URL must be at least 10 characters long");
    }

    [Fact]
    public async Task ValidateAsync_WhenLongUrlTooLong_ShouldFailWithError()
    {
        // Arrange
        var longText = "";
        while(longText.Length < 1002)
        {
            longText += "s";
        }
        var request = new ApiDataRequest()
        {
            LongUrl = longText
        };

        // Act
        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ElementAt(0).ErrorMessage.ShouldBe("The long URL must not exceed 1000 characters");
    }

    [Fact]
    public async Task ValidateAsync_WhenValidInput_ShouldSucceedWithoutError()
    {
        // Arrange
        var request = new ApiDataRequest()
        {
            LongUrl = "https://shortUrlGenerator.co.za/ThisIsALongEnoughUrlBySketch"
        };

        // Act
        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Errors.ShouldBeEmpty();
        result.IsValid.ShouldBeTrue();
    }
}
