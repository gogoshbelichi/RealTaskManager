using Microsoft.AspNetCore.Identity;
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
            const string usernameTaken = "Username is already taken.";
            const string emailTaken = "Account with this email address is already exists.";
            const string usernameAndEmailTaken = usernameTaken + " " + emailTaken;
            // Validating request
            var userByEmail = await userManager.FindByEmailAsync(request.Email);
            var userByUsername = await userManager.FindByNameAsync(request.Username);

            if (userByEmail is not null && userByUsername is not null)
            {
                return Results.BadRequest(usernameAndEmailTaken);
            }
            if (userByUsername is not null)
            {
                return Results.BadRequest(usernameTaken);
            }
            if (userByEmail is not null)
            {
                return Results.BadRequest(emailTaken);
            }
            
            await userService.CreateUser(request);
            
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
            const string roleDoesNotExist = "Role does not exist";
            // validating role existence
            if (await roleManager.RoleExistsAsync(request.Role) is false)
            {
                return Results.BadRequest(roleDoesNotExist);
            }
            // validating user existence
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return Results.BadRequest("User not found");
            //role assignment
            await userService.AssignRole(request, user);
            
            return Results.Accepted();
        }).RequireAuthorization();

        app.MapPost("/custom-refresh-token", async (
            GetRefreshTokenRequest request,
            TokenService tokenService,
            RealTaskManagerDbContext dbContext,
            CancellationToken ct) =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var user = await tokenService.TryGetUserByRefreshToken(request, ct);
                if (user is null) return Results.Unauthorized();
                var jti = Guid.NewGuid().ToString();
                var accessToken =  await tokenService.GenerateAccessToken(jti, user);
                var refreshToken = await tokenService.GenerateRefreshToken(jti, user, ct);
            
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                    return Results.BadRequest("Authentication server internal problem, please try again later");

                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return Results.Ok(new TokensResponse(accessToken, refreshToken));
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