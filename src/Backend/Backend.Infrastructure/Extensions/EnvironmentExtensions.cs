using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Extensions;

public static class EnvironmentExtensions
{
    public const string LocalEnvironmentName = "Local";
    public const string QAEnvironmentName = "QA";

    public static IReadOnlyList<string> AllowedEnvironmentNames { get; } =
        [
            LocalEnvironmentName,
            Environments.Development,
            QAEnvironmentName,
            Environments.Staging,
            Environments.Production
        ];

    extension(string? environmentName)
    {
        public string? GetCanonicalEnvironmentName() =>
            AllowedEnvironmentNames.FirstOrDefault(name => string.Equals(name, environmentName, StringComparison.OrdinalIgnoreCase));

        public bool IsAllowedEnvironmentName() =>
            environmentName.GetCanonicalEnvironmentName() is not null;
    }

    extension(IHostEnvironment hostEnvironment)
    {
        public bool IsLocal() =>
            hostEnvironment.IsEnvironment(LocalEnvironmentName);
        public bool IsQa() =>
            hostEnvironment.IsEnvironment(QAEnvironmentName);
        public bool IsTesting() =>
            hostEnvironment.IsQa();
        public bool IsDevelopment() =>
            hostEnvironment.IsDevelopment();
        public bool IsLocalOrDevelopment() =>
            hostEnvironment.IsLocal() || hostEnvironment.IsDevelopment();
        public bool IsDevelopmentOrLocal() =>
            hostEnvironment.IsLocalOrDevelopment();
        public bool IsLocalOrDevelopmentOrTesting() =>
            hostEnvironment.IsLocalOrDevelopment() || hostEnvironment.IsTesting();
        public bool IsDebugEnvironment() =>
            hostEnvironment.IsLocalOrDevelopmentOrTesting();
        public bool IsStaging() =>
            hostEnvironment.IsEnvironment(Environments.Staging);
        public bool IsProduction() =>
            hostEnvironment.IsEnvironment(Environments.Production);

        public string GetRequiredCanonicalEnvironmentName()
        {
            var aspNetCoreEnvironmentName = GetNormalizedEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var dotnetEnvironmentName = GetNormalizedEnvironmentVariable("DOTNET_ENVIRONMENT");

            if(aspNetCoreEnvironmentName is null && dotnetEnvironmentName is null)
            {
                throw new ApplicationException($"A hosting environment must be explicitly set. Allowed values: {string.Join(", ", AllowedEnvironmentNames)}.");
            }

            if(aspNetCoreEnvironmentName is not null &&
                dotnetEnvironmentName is not null &&
                !string.Equals(aspNetCoreEnvironmentName, dotnetEnvironmentName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ApplicationException($"ASPNETCORE_ENVIRONMENT ({aspNetCoreEnvironmentName}) and DOTNET_ENVIRONMENT ({dotnetEnvironmentName}) must match.");
            }

            return hostEnvironment.EnvironmentName.GetCanonicalEnvironmentName() ??
                throw new ApplicationException($"Unsupported hosting environment. Allowed values: {string.Join(", ", AllowedEnvironmentNames)}.");
        }
    }

    private static string? GetNormalizedEnvironmentVariable(string variableName) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(variableName))
            ? null
            : Environment.GetEnvironmentVariable(variableName)!.Trim();
}
