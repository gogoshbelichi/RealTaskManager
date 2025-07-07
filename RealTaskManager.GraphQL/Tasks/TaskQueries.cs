using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[QueryType]
public static class TaskQueries
{
    [Authorize]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<TaskEntity> GetTasks(RealTaskManagerDbContext dbContext)
    {
        return dbContext.Tasks.AsNoTracking().OrderBy(t => t.Title).ThenBy(t => t.Id);
    }
    
    [NodeResolver]
    public static async Task<TaskEntity?> GetTaskByIdAsync(
        Guid id,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    public static async Task<IEnumerable<TaskEntity?>> GetTasksByIdAsync(
        [ID<TaskEntity>] Guid[] ids,
        ITaskByIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}