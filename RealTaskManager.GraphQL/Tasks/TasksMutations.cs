using System.Security.Claims;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
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
        ClaimsPrincipal claimsPrincipal,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title)) throw new TitleEmptyException();
        
        var task = new TaskEntity
        {
            Title = input.Title,
            Description = input.Description,
            Status = input.Status ?? TaskStatusEnum.Backlog,
        };

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == claimsPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier), cancellationToken) ?? throw new UserNotFoundException();
        
        await dbContext.Tasks.AddAsync(task, cancellationToken);
        
        task.TasksCreatedByUser.Add(new TasksCreatedByUser(){ Task = task, User = user});

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
    
    [Authorize]
    [Error<TaskNotFoundException>]
    public static async Task<TaskEntity> UpdateTaskAsync(
        UpdateTaskInput input,
        ClaimsPrincipal claimsPrincipal,
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

        if (claimsPrincipal is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == claimsPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier), cancellationToken) ?? throw new UserNotFoundException();
        
        user.TasksAssignedToUser.Add(new TasksAssignedToUser(){ Task = task, User = user});
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    [Authorize]
    [Error<TaskNotFoundException>]
    public static async Task<DeleteTaskPayload> DeleteTaskAsync(
        DeleteTaskInput input,
        ClaimsPrincipal claimsPrincipal,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }
        
        if (claimsPrincipal is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == claimsPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier), cancellationToken) ?? throw new UserNotFoundException();

        var taskIdByUserAndTaskId = task.TasksCreatedByUser
            .Where(t => t.TaskId == task.Id && t.UserId == user.Id)
            .Select(t => t.Task.Id).FirstOrDefault();


        if (claimsPrincipal.IsInRole("Administrator") || (!claimsPrincipal.IsInRole("Administrator") && task.Id == taskIdByUserAndTaskId))
        {
            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteTaskPayload("The task has been deleted");
        }
        return new DeleteTaskPayload("The task is not deleted");
    }
    
    [Authorize]
    [Error<TaskNotFoundException>]
    public static async Task<TaskEntity> TakeTaskAsync(
        TakeTaskInput input,
        RealTaskManagerDbContext dbContext,
        ClaimsPrincipal?  claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }

        if (claimsPrincipal is null)
        {
            throw new UserNotFoundException();
        }
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == claimsPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier), cancellationToken) ?? throw new UserNotFoundException();
        
        task.TasksAssignedToUser.Add(new TasksAssignedToUser { Task = task, User = user});
        task.Status = TaskStatusEnum.InProgress;

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
}