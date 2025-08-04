using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[QueryType]
public static class TaskQueries
{
    //[Authorize]
    [NodeResolver]
    public static async Task<TaskEntity?> GetTaskByIdAsync(
        Guid id,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    //[Authorize]
    public static async Task<IEnumerable<TaskEntity>> GetTasksByIdAsync(
        [ID<TaskEntity>] Guid[] ids,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
    
    //[Authorize]
    [UseFiltering/*(typeof(TaskFilter))*/]
    [UseSorting]
    public static async Task<PageConnection<TaskEntity>> GetTasks(
        PagingArguments args,
        QueryContext<TaskEntity> query,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct)
    {
        var page = await dbContext.Tasks.With(query).OrderBy(t => t.Id).ToPageAsync(args, ct);

        return new PageConnection<TaskEntity>(page);
    }
}