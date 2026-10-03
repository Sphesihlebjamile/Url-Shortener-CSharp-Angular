namespace Backend.Application.Exceptions;

/// <summary>
/// Represents a custom application exception with additional properties for status code, title, and error messages 🦆
/// </summary>
public interface IApplicationException
{
    /// <summary>
    /// Represents the HTTP status code associated with the exception.
    /// This makes it easy for the Global Exception Handler to map
    /// the exception to an appropriate HTTP response 🦆
    /// </summary>
    public int StatusCode { get; set; }
    /// <summary>
    /// Title of the exception that will eventually surface to the consuming client 🦆
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// A list of error messages associated with the exception.
    /// This allows for multiple errors to be communicated in a single response.
    /// Should be generally used to validation exceptions, or to provide detailed error information
    /// if required 🦆
    /// </summary>
    public IReadOnlyList<string> Errors { get; set; }
}
