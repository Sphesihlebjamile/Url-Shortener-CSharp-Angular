using Backend.Api.Validation.DataEndpoints;
using FluentValidation;

namespace Backend.Api.Configuration;

public static class ServiceCollectionRegistration
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddValidation();

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApiDataRequestValidator>();

        return services;
    }
}
