using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        services.AddDbContext<RealTaskManagerDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        
        /*services.AddDbContext<CustomIdentityDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));*/
        
        return services;
    }
}