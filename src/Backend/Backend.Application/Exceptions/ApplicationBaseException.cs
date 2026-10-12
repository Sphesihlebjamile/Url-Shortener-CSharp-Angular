namespace Backend.Application.Exceptions;

/// <summary>
/// The base class for all application-specific exceptions.
/// It provides the structure for custom exceptions with additional properties like StatusCode, Title, and Errors 🦆
/// </summary>
public abstract class ApplicationBaseException : Exception, IApplicationException
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class with a specified error message and status code 🦆
    /// </summary>
    /// <param name="title">The title of the exception.</param>
    /// <param name="message">The message of the exception.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    protected ApplicationBaseException(string title, string message, int statusCode)
        : base(message)
    {
        Title = title;
        StatusCode = statusCode;
    }
}
