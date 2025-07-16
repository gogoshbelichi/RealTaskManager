using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealTaskManager.UseCases.Authentication.RefreshTokens;

namespace RealTaskManager.UseCases;

public static class UseCasesServiceExtensions
{
    public static IServiceCollection AddUseCasesServices(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        services.AddScoped<AddRefreshTokenHandler>();
        services.AddScoped<GetRefreshTokenHandler>();
        
        return services;
    }
}