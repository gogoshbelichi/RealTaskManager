using HotChocolate.Authorization;
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
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title)) throw new TitleEmptyException();
        
        var task = new TaskEntity
        {
            Title = input.Title,
            Description = input.Description,
            Status = input.Status ?? TaskStatusEnum.Backlog
        };

        await dbContext.Tasks.AddAsync(task, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
    
    [Authorize]
    [Error<TitleEmptyException>]
    public static async Task<TaskEntity> UpdateTaskAsync(
        UpdateTaskInput input,
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
    
    [Authorize]
    [Error<TitleEmptyException>]
    public static async Task<TaskEntity> TakeTaskAsync(
        TakeTaskInput input,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], cancellationToken);

        if (task is null)
        {
            throw new TaskNotFoundException();
        }

        var user = new UserEntity
        {
            Username = "gogosh",
            Email = "gogosh@ex.com",
            
            
        };
        task.TasksAssignedToUser.Add(new TasksAssignedToUser { Task = task, User = user});
        task.Status = TaskStatusEnum.InProgress;

        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }
}