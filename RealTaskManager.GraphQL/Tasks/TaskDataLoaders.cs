using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AssignedTasks;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

public static class TaskDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, TaskEntity>> TaskByIdAsync(
        IReadOnlyList<Guid> ids,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => s.Id, selector)
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }
    
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Page<TasksAssignedToUser>>> UsersAssignedToTasksAsync(
        IReadOnlyList<Guid> taskIds,
        RealTaskManagerDbContext dbContext,
        QueryContext<TasksAssignedToUser> query,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        return await dbContext.TasksAssignedToUsers
            .AsNoTracking()
            .Where(s => taskIds.Contains(s.TaskId))
            .With(query, AssignedTasksOrdering.TasksAssignedDefaultOrder)
            .ToBatchPageAsync(s => s.TaskId, args, cancellationToken);
    }
}