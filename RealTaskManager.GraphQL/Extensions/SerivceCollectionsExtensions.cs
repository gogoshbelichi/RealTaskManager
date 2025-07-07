using Microsoft.AspNetCore.Identity;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Extensions;

public static class SerivceCollectionsExtensions
{
    public static WebApplicationBuilder AddCustomIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCore<TaskManagerUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<CustomIdentityDbContext>()
            .AddApiEndpoints()
            .AddDefaultTokenProviders();
        
        return builder;
    }

    public static async Task<WebApplication> UseCustomRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (await roleManager.FindByNameAsync("Administrator") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("Administrator"));
        }
        if (await roleManager.FindByNameAsync("User") ==  null)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        return app;
    }
}