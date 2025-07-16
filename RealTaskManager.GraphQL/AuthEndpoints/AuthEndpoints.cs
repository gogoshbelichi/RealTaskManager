using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;
using RealTaskManager.UseCases.Authentication.RefreshTokens;

namespace RealTaskManager.GraphQL.AuthEndpoints;

public static class AuthEndpoints
{
    /// <summary>
    /// Custom auth endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/custom-login", async (
            CustomLoginRequest request,
            TokenService service,
            UserManager<TaskManagerUser> userManager,
            CancellationToken ct) =>
        {
            var user = await userManager.FindByEmailAsync(request.Login) ?? await userManager.FindByNameAsync(request.Login);
            if (user is null) return Results.BadRequest("User not found");
            
            if (!await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Unauthorized();
            
            var jti = Guid.NewGuid().ToString();
            var access_token =  await service.GenerateAccessToken(jti, user);
            var refresh_token = await service.GenerateRefreshToken(jti, user, ct);
            
            if (string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
                return Results.BadRequest("Authentication server internal problem");
            
            return Results.Ok(new TokensResponse(access_token, refresh_token));
        });

        app.MapPost("/custom-register", async (
            CustomUserService userService,
            UserManager<TaskManagerUser> userManager,
            CustomRegisterRequest request) =>
        {
            // Validating request
            var userByEmail = await userManager.FindByEmailAsync(request.Email);
            var userByUsername = await userManager.FindByNameAsync(request.Username);

            if (userByEmail is not null && userByUsername is not null)
            {
                return Results.BadRequest(
                    "Username is already taken and account with this email address is already exists.");
            }
            if (userByUsername is not null)
            {
                return Results.BadRequest("Username already taken.");
            }
            if (userByEmail is not null)
            {
                return Results.BadRequest("You cant register account with this email address.");
            }
            
            // in AspNetUsers table
            var createUserTaskCompletionSource = new TaskCompletionSource<bool>();
            // in Users table
            var createProfileTaskCompletionSource = new TaskCompletionSource<bool>();
            
            await userService.CreateUser(request, createUserTaskCompletionSource, createProfileTaskCompletionSource);
            
            if (createUserTaskCompletionSource.Task.Result is false ||
                createProfileTaskCompletionSource.Task.Result is false)
            {
                return Results.BadRequest("Registration failed");
            }
            
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
            AssignRoleRequest request,
            UserManager<TaskManagerUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CustomUserService userService
            ) =>
        {
            // in AspNetUsers table
            var assignRoleTaskCompletionSource = new TaskCompletionSource<bool>();
            // in Users table
            var mapRoleTaskCompletionSource = new TaskCompletionSource<bool>();
            // validating role existence
            if (await roleManager.RoleExistsAsync(request.Role) is false)
            {
                return Results.BadRequest("Role does not exist");
            }
            // validating user existence
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return Results.BadRequest("User not found");
            //role assignment
            await userService.AssignRole(request, user, assignRoleTaskCompletionSource, mapRoleTaskCompletionSource);
            
            return assignRoleTaskCompletionSource.Task.Result && mapRoleTaskCompletionSource.Task.Result ?
                Results.Accepted() : Results.BadRequest("Role is not assigned");
        }).RequireAuthorization();

        app.MapPost("/custom-refresh-token", async (
            GetRefreshTokenRequest request,
            TokenService tokenService,
            CustomIdentityDbContext dbContext,
            CancellationToken ct) =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var user = await tokenService.TryGetUserByRefreshToken(request, ct);
                if (user is null) return Results.Unauthorized();
                var jti = Guid.NewGuid().ToString();
                var access_token =  await tokenService.GenerateAccessToken(jti, user);
                var refresh_token = await tokenService.GenerateRefreshToken(jti, user, ct);
            
                if (string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
                    return Results.BadRequest("Authentication server internal problem, please try again later");

                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return Results.Ok(new TokensResponse(access_token, refresh_token));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                return Results.BadRequest("Authentication server internal problem, please try again later");
            }
        });
        
        return app;
    }
}