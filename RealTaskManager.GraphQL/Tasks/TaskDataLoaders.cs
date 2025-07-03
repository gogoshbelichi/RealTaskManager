using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

public static class TaskDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, TaskEntity>> TaskByTaskIdAsync(
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
    public static async Task<IReadOnlyDictionary<Guid, TaskEntity[]>> TasksCreatedByUserIdAsync(
        IReadOnlyList<Guid> userIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(s => userIds.Contains(s.Id))
            .Select(s => s.Id, s => s.TasksCreatedByUser.Select(ss => ss.Task), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }
    
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, TaskEntity[]>> TasksAssignedToUserIdAsync(
        IReadOnlyList<Guid> userIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(s => userIds.Contains(s.Id))
            .Select(s => s.Id, s => s.TasksAssignedToUser.Select(ss => ss.Task), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }
}