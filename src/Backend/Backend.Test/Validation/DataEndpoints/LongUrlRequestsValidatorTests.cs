namespace Backend.Test.Validation.DataEndpoints;

public class LongUrlRequestsValidatorTests
{
    [Theory]
    [InlineData("r")]
    [InlineData("3tg")]
    [InlineData("28fd")]
    [InlineData("SDF3")]
    [InlineData("VBA342")]
    public void ValidateShortUrlCode_WhenValidShortUrlCode_ShouldSucceed(string validShortUrlCode)
    {
        // Arrange & Act
        var result = LongUrlRequestValidator.ValidateShortUrlCode(validShortUrlCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBe(true);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateShortUrlCode_WhenEmptyShortUrlCode_ShouldFailWithErrorMessage()
    {
        // Arrange
        string invalidShortUrlCode = string.Empty;

        // Act
        var result = LongUrlRequestValidator.ValidateShortUrlCode(invalidShortUrlCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ElementAt(0).ErrorMessage.ShouldBe("A short url code is required");
    }

    [Fact]
    [InlineData(null)]
    public void ValidateShortUrlCode_WhenNullShortUrlCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string? invalidShortUrlCode = null;

        // Act & Assert
        var result = Should.Throw<InvalidOperationException>(
            () => LongUrlRequestValidator.ValidateShortUrlCode(invalidShortUrlCode));
        result.ShouldNotBeNull();
        result.Source.ShouldBe("FluentValidation");
    }

    [Theory]
    [InlineData("sdfsgsdfhy437567456")]
    [InlineData("sdfsgsdfhy43756745687075356yhfgfdgh")]
    [InlineData("sdfsgsd4")]
    public void ValidateShortUrlCode_WhenInvalidShortUrlCode_ShouldFail(string invalidShortUrlCode)
    {
        // Arrange & Act
        var result = LongUrlRequestValidator.ValidateShortUrlCode(invalidShortUrlCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ElementAt(0).ErrorMessage.ShouldBe("A short url should have a maximum of 7 characters");
    }
}
