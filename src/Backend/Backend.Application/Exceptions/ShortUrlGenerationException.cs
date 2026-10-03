namespace Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when there is a failure in generating a short URL 🦆
/// </summary>
public class ShortUrlGenerationException : ApplicationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortUrlGenerationException"/> class with a specified error message 🦆
    /// </summary>
    /// <param name="message">The error message.</param>
    public ShortUrlGenerationException(string message) 
        : base("Failed to generate short URL", message, 500) // Default status code to 500 (Internal Server Error)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortUrlGenerationException"/> class with a specified error message and status code 🦆
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public ShortUrlGenerationException(string message, int statusCode) 
        : base("Failed to generate short URL", message, statusCode)
    {
    }
}
