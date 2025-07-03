using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[QueryType]
public static class TaskQueries
{
    [UsePaging]
    public static IQueryable<TaskEntity> GetTracks(RealTaskManagerDbContext dbContext)
    {
        return dbContext.Tasks.AsNoTracking().OrderBy(t => t.Title).ThenBy(t => t.Id);
    }
    
    [NodeResolver]
    public static async Task<TaskEntity?> GetTaskByIdAsync(
        Guid id,
        ITaskByTaskIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    public static async Task<IEnumerable<TaskEntity?>> GetTasksByIdAsync(
        Guid[] ids,
        ITaskByTaskIdDataLoader taskById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await taskById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}