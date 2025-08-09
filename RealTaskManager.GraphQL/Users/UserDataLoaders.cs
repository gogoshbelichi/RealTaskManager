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
        Console.WriteLine("Dataloader UserByIdAsync");
        return await dbContext.UserProfiles
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .Select(a => a.Id, selector)
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
    
    [DataLoader]
    public static async Task<Page<UserEntity>> PagedUsersByIdAsync(
        IReadOnlyList<Guid> ids,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Dataloader UserByIdAsync");
        return await dbContext.UserProfiles
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .OrderBy(a => a.Id)
            .Select(a => a.Id, selector)
            .ToPageAsync(args, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Page<TaskEntity>>> TasksCreatedByUserAsync(
        IReadOnlyList<Guid> userIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Where(s => userIds.Contains(s.CreatedByUserId))
            .OrderBy(s => s.Id)
            .Select(s => s.CreatedByUserId, selector)
            .ToBatchPageAsync(s => s.CreatedByUserId, args, cancellationToken);
    }
    
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Page<TasksAssignedToUser>>> TasksAssignedToUsersAsync(
        IReadOnlyList<Guid> userIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        return await dbContext.TasksAssignedToUsers
            .AsNoTracking()
            .Where(s => userIds.Contains(s.UserId))
            .OrderBy(s => s.UserId)
            .Select(ss => ss.UserId, selector)
            .ToBatchPageAsync(s => s.UserId, args, cancellationToken);
    }
}