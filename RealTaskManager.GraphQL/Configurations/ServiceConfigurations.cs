using RealTaskManager.Infrastructure;

namespace RealTaskManager.GraphQL.Configurations;

public static class ServiceConfigurations
{
    public static IServiceCollection AddServiceConfigs(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddInfrastructureServices(builder.Configuration);
        
        return services;
    }
}