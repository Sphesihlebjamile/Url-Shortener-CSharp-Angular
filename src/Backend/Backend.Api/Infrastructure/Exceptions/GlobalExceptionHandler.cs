namespace Backend.Api.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(IHostEnvironment env)
    {
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error has occurred!",
            Detail = GetSafeExceptionMessage(exception, httpContext),
            Instance = httpContext.Request.Path
        };

        if (exception is IApplicationException contractPipelineException)
        {
            problemDetails.Status = contractPipelineException.StatusCode;
            problemDetails.Title = contractPipelineException.Title;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }

    private string GetSafeExceptionMessage(Exception exception, HttpContext context)
    {
        // Only expose details in development
        if (_env.IsDevelopment())
        {
            return exception.Message;
        }

        return "An unexpected error occurred.";
    }
}
