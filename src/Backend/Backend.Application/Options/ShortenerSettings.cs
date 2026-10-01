namespace Backend.Application.Options;

public class ShortenerSettings
{
    public const string SectionName = "ShortenerSettings";

    public required string ApplicationUrl { get; set; }
}
