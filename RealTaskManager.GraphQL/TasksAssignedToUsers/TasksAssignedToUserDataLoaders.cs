using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.TasksAssignedToUsers;

public class TasksAssignedToUserDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Page<TasksAssignedToUser>>> TasksAssignedToUserByTaskIdAsync(
        IReadOnlyList<Guid> taskIds,
        RealTaskManagerDbContext dbContext,
        ISelectorBuilder selector,
        PagingArguments args,
        CancellationToken ct) 
    {
        Console.WriteLine("Dataloader TasksAssignedToUserByTaskIdAsync");
        return await dbContext.TasksAssignedToUsers
            .AsNoTracking()
            .Where(a => taskIds.Contains(a.TaskId))
            .OrderBy(u => u.TaskId)
            .Select(a => a.TaskId, selector)
            .ToBatchPageAsync(a => a.TaskId, args, ct);
    }
}