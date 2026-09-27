using Backend.Api.Validation.DataEndpoints;
using Backend.Application.Orchestrators;
using Backend.Application.Plans;
using Backend.Infrastructure.Orchestrators;
using Backend.Infrastructure.Plans;
using FluentValidation;

namespace Backend.Api.Configuration;

public static class ServiceCollectionRegistration
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddValidation();
        services.AddApplication();

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApiDataRequestValidator>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUrlShortenerOrchestrator), typeof(UrlShortenerOrchestrator));
        services.AddScoped(typeof(IUrlShortenerPlan), typeof(UrlShortenerPlan));

        return services;
    }
}
