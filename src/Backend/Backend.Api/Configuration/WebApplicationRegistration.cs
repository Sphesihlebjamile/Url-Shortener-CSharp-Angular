namespace Backend.Api.Configuration;

[ExcludeFromCodeCoverage]
public static class WebApplicationRegistration
{
    public static WebApplication UseEndpoints(this WebApplication app)
    {
        app.MapDataEndpoints();

        return app;
    }
}
