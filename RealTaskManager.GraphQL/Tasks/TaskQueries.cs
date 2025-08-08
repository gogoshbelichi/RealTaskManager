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
    [UsePaging, UseFiltering(typeof(TaskFilterInputType)), UseSorting(typeof(TasksSorting))]
    public static async Task<Connection<TaskEntity>> GetTasksV2Async(
        RealTaskManagerDbContext dbContext,
        PagingArguments args,
        QueryContext<TaskEntity>? query = default,
        CancellationToken ct = default)
    {
        Console.WriteLine("TaskQueries GetTasksV2");
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