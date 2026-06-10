using SentinelAML.Application;
using SentinelAML.Infrastructure;
using SentinelAML.Persistence;

namespace SentinelAML.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSentinelAMLServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddPersistence(configuration);
        services.AddInfrastructure(configuration);

        return services;
    }
}
