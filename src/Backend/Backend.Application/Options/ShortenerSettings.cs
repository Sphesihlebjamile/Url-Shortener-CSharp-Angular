namespace Backend.Application.Options;

public class ShortenerSettings
{
    public const string SectionName = "ShortenerSettings";

    public required string ApiApplicationUrl { get; set; }
    public required string ClientApplicationHomeUrl { get; set; }
}
