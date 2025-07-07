using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
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
    public static async Task<IReadOnlyDictionary<Guid, UserEntity[]>> UsersCreatedTasksAsync(
        IReadOnlyList<Guid> taskIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(s => taskIds.Contains(s.Id))
            .Select(s => s.Id, s => s.TasksCreatedByUser
                .Select(ss => ss.User), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value
                .ToArray(), cancellationToken);
    }
    
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, UserEntity[]>> UsersAssignedToTasksAsync(
        IReadOnlyList<Guid> taskIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(s => taskIds.Contains(s.Id))
            .Select(s => s.Id, s => s.TasksAssignedToUser
                .Select(ss => ss.User), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value
                .ToArray(), cancellationToken);
    }
}