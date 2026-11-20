using Microsoft.Extensions.FileProviders;

namespace Backend.Test.Setup
{
    public sealed class FakeHostEnvironment(string? environmentName = null) : IHostEnvironment
    {
        public string ApplicationName { get; set; } = "UrlShortener";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public string EnvironmentName { get; set; } = environmentName ?? Environments.Development;
    }
}
