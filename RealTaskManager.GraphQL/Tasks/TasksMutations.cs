using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[MutationType]
public class TasksMutations
{
    [Error<TitleEmptyException>]
    public static async Task<TaskEntity> AddTaskAsync(
        AddTaskInput input,
        RealTaskManagerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title)) throw new TitleEmptyException();
        
        var speaker = new TaskEntity
        {
            Title = input.Title,
            Description = input.Description,
            Status = input.Status ?? "Todo"
        };

        await dbContext.Tasks.AddAsync(speaker, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return speaker;
    }
}