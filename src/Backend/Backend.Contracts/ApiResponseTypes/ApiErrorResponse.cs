namespace Backend.Contracts.ApiResponseTypes;

public class ApiErrorResponse
{
    public int Status { get; set; }
    public required string Title { get; set; }
    public required string Instance { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];
}
