using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SentinelAML.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register external service implementations here.
        return services;
    }
}
