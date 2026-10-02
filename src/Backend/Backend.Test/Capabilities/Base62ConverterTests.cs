using Backend.Infrastructure.Capabilities;
using Shouldly;

namespace Backend.Test.Capabilities;

public class Base62ConverterTests
{
    [Theory]
    [InlineData(1, "1")]
    [InlineData(75, "1D")]
    [InlineData(500, "84")]
    [InlineData(1500, "OC")]
    [InlineData(1000000, "4C92")]
    [InlineData(3904342542, "4GEDqo")]
    [InlineData(948202387138, "Gh0KaHK")]
    [InlineData(54894513245597, "FaRolHMz")]
    public void Base62Converter_ShouldReturnCorrectBase62String(
        long base10Value, string expectedBase62String)
    {
        // Arrange
        var converter = new Base62Converter();

        // Act
        string actualBase62Output = converter.Execute(base10Value);

        // Assert
        actualBase62Output.ShouldBe(expectedBase62String);
    }

    [Fact]
    public void Base62Converter_ShouldReturnZeroForZeroInput()
    {
        // Arrange
        var converter = new Base62Converter();
        long base10Input = 0;
        string expectedBase62Output = "0";

        // Act
        string actualBase62Output = converter.Execute(base10Input);

        // Assert
        actualBase62Output.ShouldBe(expectedBase62Output);
    }

    [Fact]
    public void Base62Conver_ShouldThrowArgumentOutOfRangeExceptionForNegativeInput()
    {
        // Arrange
        var converter = new Base62Converter();
        long base10Input = -1;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            converter.Execute(base10Input);
        });
    }
}
