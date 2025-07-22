using System.Security.Claims;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[MutationType]
public static class TasksMutations
{
    [Authorize]
    [Error<TitleEmptyException>]
    public static async Task<TaskEntity> AddTaskAsync(
        AddTaskInput input,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title)) throw new TitleEmptyException();
        
        var username = claims.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);
        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == username, cancellationToken) 
                   ?? throw new UserNotFoundException();
        
        var task = new TaskEntity
        {
            Title = input.Title,
            Description = input.Description,
            Status = input.Status ?? TaskStatusEnum.Backlog,
            CreatedBy = user,
            CreatedByUserId = user.Id,
        };
        
        await dbContext.Tasks.AddAsync(task, cancellationToken);

        if (input.AssignToUserByUsername is not null && claims.IsInRole("Administrator"))
        {
            var assigned = dbContext.UserProfiles.FirstOrDefault(u => u.Username == input.AssignToUserByUsername);
            if (assigned is null) throw new UserNotFoundException();
            task.TasksAssignedToUser.Add(new TasksAssignedToUser(){ Task = task, User = assigned });
        }
        

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
    
    [Authorize]
    [Error<TaskNotFoundException>]
    public static async Task<TaskEntity> UpdateTaskAsync(
        UpdateTaskInput input,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }

        task.Title = input.Title ?? task.Title;
        task.Description = input.Description ?? task.Description;
        task.Status = input.Status ?? task.Status;

        if (claims is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == claims
            .FindFirstValue(JwtRegisteredClaimNames.PreferredUsername), cancellationToken) ?? throw new UserNotFoundException();
        
        user.TasksAssignedToUser.Add(new TasksAssignedToUser(){ Task = task, User = user});
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    //[Authorize("UserPolicy")]
    [Error<TaskNotFoundException>]
    public static async Task<DeleteTaskPayload> DeleteTaskAsync(
        DeleteTaskInput input,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }
        
        if (claims is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == claims
            .FindFirstValue(JwtRegisteredClaimNames.PreferredUsername), cancellationToken) ?? throw new UserNotFoundException();


        if (claims.IsInRole("Administrator") || (!claims.IsInRole("Administrator") && task.CreatedByUserId == user.Id))
        {
            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteTaskPayload("The task has been deleted");
        }
        return new DeleteTaskPayload("The task is not deleted");
    }
    
    //[Authorize("UserPolicy")]
    [Error<TaskNotFoundException>]
    public static async Task<TaskEntity> TakeTaskAsync(
        TakeTaskInput input,
        RealTaskManagerDbContext dbContext,
        ClaimsPrincipal claims,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }

        if (claims is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == claims
            .FindFirstValue(JwtRegisteredClaimNames.PreferredUsername), cancellationToken) ?? throw new UserNotFoundException();
        
        task.TasksAssignedToUser.Add(new TasksAssignedToUser { Task = task, User = user});
        task.Status = TaskStatusEnum.InProgress;

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
}