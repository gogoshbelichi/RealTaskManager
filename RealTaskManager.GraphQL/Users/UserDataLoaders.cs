using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Users;

public class UserDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, UserEntity>> UserByIdAsync(
        IReadOnlyList<Guid> ids,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .Select(a => a.Id, selector)
            .ToDictionaryAsync(a => a.Id, cancellationToken);
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