namespace Backend.Domain.Entities;

/// <summary>
/// Represents a URL entity with properties for short and long URLs.
/// </summary>
public class Url
{
    /// <summary>
    /// The unique identifier for the URL entity.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The shortened version of the URL.
    /// </summary>
    public required string ShortUrl { get; set; }
    /// <summary>
    /// The original long version of the URL.
    /// </summary>
    public required string LongUrl { get; set; }
}
