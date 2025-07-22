using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AuthEndpoints;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL;

public class CustomUserService(
    UserManager<TaskManagerUser> userManager,
    RealTaskManagerDbContext dbContext)
{
    /// <summary>
    /// User registration case
    /// </summary>
    /// <param name="request">register data</param>
    /// <param name="createUserTaskCompletionSource">AspNetUser creation result source</param>
    /// <param name="createProfileTaskCompletionSource">User profile creation result source</param>
    /// <exception cref="Exception">Any creation exception</exception>
    public async Task CreateUser(CustomRegisterRequest request)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var user = new TaskManagerUser { Email = request.Email,  UserName = request.Username };
            
            var creation = await userManager.CreateAsync(user, request.Password);
            
            var assignement = await userManager.AddToRoleAsync(user, "User");

            if (creation.Succeeded is false || assignement.Succeeded is false)
            {
                throw new Exception("User creation failed, transaction rollback");
            }

            await CreateProfile(request);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
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
    private async Task CreateProfile(CustomRegisterRequest request)
    {
        var userEntity = new UserEntity
        {
            Email = request.Email,
            Username = request.Username,
        };
        userEntity.Roles.Add("User"); 
        await dbContext.UserProfiles.AddAsync(userEntity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AssignRole(
        AssignRoleRequest request,
        TaskManagerUser user)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await userManager.AddToRoleAsync(user, request.Role);
            
            if (result.Succeeded is false)
            {
                throw new Exception($"Role is not assigned");
            }

            await MapRoleToProfile(request);
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
    
    public async Task MapRoleToProfile(AssignRoleRequest request)
    {
        var user = await dbContext.UserProfiles
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync();
        
        if (user is null) throw new NullReferenceException("User profile not found");
        
        user.Roles.Add(request.Role);
        await dbContext.SaveChangesAsync();
    }
}