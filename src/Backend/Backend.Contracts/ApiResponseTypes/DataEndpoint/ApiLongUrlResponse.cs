namespace Backend.Contracts.ApiResponseTypes.DataEndpoint;

public sealed class ApiLongUrlResponse
{
    public required string LongUrl { get; set; }
    public long ServerTime { get; set; }
}
