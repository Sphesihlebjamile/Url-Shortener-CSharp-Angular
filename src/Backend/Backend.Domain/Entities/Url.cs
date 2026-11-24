namespace Backend.Domain.Entities;

/// <summary>
/// Represents a URL entity with properties for short and long URLs.
/// </summary>
public class Url
{
    /// <summary>
    /// The unique identifier for the URL entity.
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// The shortened version of the URL.
    /// </summary>
    public required string ShortUrl { get; set; }
    /// <summary>
    /// The original long version of the URL.
    /// </summary>
    public required string LongUrl { get; set; }
    /// <summary>
    /// Indicates whether the entity is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    /// <summary>
    /// The date and time at which the URL expires.
    /// Defaults to null as the default behaviour is for URLs to not expire.
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; } = null;
    /// <summary>
    /// The date at which an entry was created.
    /// </summary>
    public required DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    /// <summary>
    /// The date at which the entry was last updated
    /// </summary>
    public required DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;
}
