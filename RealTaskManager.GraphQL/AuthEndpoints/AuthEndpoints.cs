using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.AuthEndpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/custom-login", async (
            LoginRequest request,
            TokenGenerator generator,
            UserManager<TaskManagerUser> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return Results.BadRequest("User not found");
            if (!await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Unauthorized();
            var userRoles = await userManager.GetRolesAsync(user);
            var userId = user.Id;
            var username = user.UserName ?? "The username is not specified";
            var access_token = generator.GenerateToken(username, request.Email, userId, userRoles);
            return Results.Ok(access_token);
        });

        app.MapPost("/custom-register", async (
            UserManager<TaskManagerUser> userManager,
            RealTaskManagerDbContext dbContext,
            RegisterRequest request, string username) =>
        {
            var user = new TaskManagerUser { Email = request.Email,  UserName = username };
            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return Results.BadRequest($"Registration failed {result.Errors}");
            }

            var userEntity = new UserEntity
            {
                IdentityId = user.Id
            };
            
            await dbContext.Users.AddAsync(userEntity);
            await dbContext.SaveChangesAsync();
            
            return Results.Created();
        });

        app.MapPost("/add-role", async (
            string role,
            RoleManager<IdentityRole> roleManager) =>
        {
            if (await roleManager.RoleExistsAsync(role)) return Results.Conflict("Role already exists");
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            return result.Succeeded ? Results.Created() : Results.BadRequest("Role not created");
        }).RequireAuthorization("AdminPolicy");

        app.MapPatch("/assign-role", async (
            string role,
            string email,
            UserManager<TaskManagerUser> userManager
            ) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            
            if (user is null) return Results.BadRequest("User not found");
            var result = await userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? Results.Accepted() : Results.BadRequest("Role not added");
        }).RequireAuthorization();
        return app;
    }
}