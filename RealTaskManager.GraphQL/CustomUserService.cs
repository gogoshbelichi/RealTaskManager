using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AuthEndpoints;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL;

public class CustomUserService(
    UserManager<TaskManagerUser> userManager,
    RealTaskManagerDbContext dbContext,
    CustomIdentityDbContext identityDbContext)
{
    /// <summary>
    /// User registration case
    /// </summary>
    /// <param name="request">register data</param>
    /// <param name="createUserTaskCompletionSource">AspNetUser creation result source</param>
    /// <param name="createProfileTaskCompletionSource">User profile creation result source</param>
    /// <exception cref="Exception">Any creation exception</exception>
    public async Task CreateUser(CustomRegisterRequest request,
        TaskCompletionSource<bool> createUserTaskCompletionSource,
        TaskCompletionSource<bool> createProfileTaskCompletionSource)
    {
        await using var transaction = await identityDbContext.Database.BeginTransactionAsync();
        try
        {
            var user = new TaskManagerUser { Email = request.Email,  UserName = request.Username };
            
            var creation = await userManager.CreateAsync(user, request.Password);
            
            var assignement = await userManager.AddToRoleAsync(user, "User");

            if (creation.Succeeded is false || assignement.Succeeded is false)
            {
                createUserTaskCompletionSource.TrySetResult(false);
                throw new Exception("User creation failed, transaction rollback");
            }

            await CreateProfile(request, user, createProfileTaskCompletionSource);
            
            if (createProfileTaskCompletionSource.Task.Result is false)
            {
                createUserTaskCompletionSource.TrySetResult(false);
                throw new Exception("Profile creation failed, transaction rollback");
            }
            
            createUserTaskCompletionSource.TrySetResult(true);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // if some other error happened - to ensure setting is false
            createUserTaskCompletionSource.TrySetResult(false);
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    /// <summary>
    /// User registration profile creation
    /// </summary>
    /// <param name="request">register data</param>
    /// <param name="newUser">TaskManagerUser instance</param>
    /// <param name="createProfileTaskCompletionSource">User profile creation result source</param>
    /// <exception cref="NullReferenceException">TaskManagerUser is null</exception>
    private async Task CreateProfile(CustomRegisterRequest request,
        TaskManagerUser? newUser,
        TaskCompletionSource<bool> createProfileTaskCompletionSource)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            if (newUser is null) throw new NullReferenceException("User creation failed");
        
            var userEntity = new UserEntity
            {
                IdentityId = newUser.Id,
                Email = request.Email,
                Username = request.Username
            };
            userEntity.Roles.Add("User"); 
            await dbContext.Users.AddAsync(userEntity);
            await dbContext.SaveChangesAsync();
            
            createProfileTaskCompletionSource.TrySetResult(true);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // if some other error happened - to ensure setting is false
            createProfileTaskCompletionSource.TrySetResult(false);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task AssignRole(
        AssignRoleRequest request,
        TaskManagerUser user, TaskCompletionSource<bool> assignRoleTaskCompletionSource,
        TaskCompletionSource<bool> mapRoleTaskCompletionSource)
    {
        await using var transaction = await identityDbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await userManager.AddToRoleAsync(user, request.Role);
            
            if (result.Succeeded is false)
            {
                assignRoleTaskCompletionSource.TrySetResult(false);
                throw new Exception($"Role is not assigned");
            }

            await MapRoleToProfile(request, mapRoleTaskCompletionSource);

            if (mapRoleTaskCompletionSource.Task.Result is false)
            {
                assignRoleTaskCompletionSource.TrySetResult(false);
                throw new Exception("Profile is not assigned");
            }
            
            assignRoleTaskCompletionSource.TrySetResult(true);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            assignRoleTaskCompletionSource.TrySetResult(false);
            await transaction.RollbackAsync();
            throw;
        }
        
    }
    
    public async Task MapRoleToProfile(AssignRoleRequest request, TaskCompletionSource<bool> mapRoleTaskCompletionSource)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var user = await dbContext.Users
                .Where(u => u.Email == request.Email)
                .FirstOrDefaultAsync();
            
            if (user is null) throw new NullReferenceException("User profile not found");
            
            user.Roles.Add(request.Role);
            await dbContext.SaveChangesAsync();
            
            mapRoleTaskCompletionSource.TrySetResult(true);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            mapRoleTaskCompletionSource.TrySetResult(false);
            await transaction.RollbackAsync();
            throw;
        }
    }
}