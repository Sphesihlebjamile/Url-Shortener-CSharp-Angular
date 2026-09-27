using Backend.Api.Endpoints;

namespace Backend.Api.Configuration;

public static class WebApplicationRegistration
{
    public static WebApplication UseEndpoints(this WebApplication app)
    {
        app.MapDataEndpoints();

        return app;
    }
}
