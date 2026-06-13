using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Provides the Application layer registration boundary for Gateway use cases and application services.
/// </summary>
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
