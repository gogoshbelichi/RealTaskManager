using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

[ObjectType<TaskEntity>]
public static partial class TaskType
{
    [BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    public static async Task<IEnumerable<TaskEntity>> GetTasksAsync(
        [Parent] TaskEntity taskEntity,
        ITasksAssignedToUserIdDataLoader assignedToByTaskEntityId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await assignedToByTaskEntityId
            .Select(selection)
            .LoadRequiredAsync(taskEntity.Id, cancellationToken);
    }
}