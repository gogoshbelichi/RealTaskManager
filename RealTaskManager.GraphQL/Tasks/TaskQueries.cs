using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[QueryType]
public static class TaskQueries
{
    [Authorize]
    [UsePaging]
    [UseFiltering(typeof(TaskFilterInputType))]
    [UseSorting]
    public static IQueryable<TaskEntity> GetTasks(RealTaskManagerDbContext dbContext)
    {
        return dbContext.Tasks.AsNoTracking().OrderBy(t => t.Title).ThenBy(t => t.Id);
    }
    
    [UsePaging, UseFiltering(typeof(TaskFilterInputType)), UseSorting]
    public static async Task<Connection<TaskEntity>> GetTasksV2Async(
        RealTaskManagerDbContext dbContext,
        PagingArguments args,
        QueryContext<TaskEntity>? query = default,
        CancellationToken ct = default)
    {
        Console.WriteLine("UserQueries GetUsers");
        return await dbContext.Tasks
            .AsNoTracking()
            .With(query, TasksOrdering.TasksDefaultOrder)
            .ToPageAsync(args, ct).ToConnectionAsync();
    }
    
    [Authorize]
    [NodeResolver]
    public static async Task<TaskEntity?> GetTaskByIdAsync(
        Guid id,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    [Authorize]
    public static async Task<IEnumerable<TaskEntity>> GetTasksByIdAsync(
        [ID<TaskEntity>] Guid[] ids,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}