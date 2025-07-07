using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

[ObjectType<TaskEntity>]
public static partial class TaskType
{
    static partial void Configure(IObjectTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(
                async (ctx, id)
                    => await ctx.DataLoader<ITaskByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }
    
    [BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    public static async Task<IEnumerable<UserEntity>> GetUsersAssignedToTasksAsync(
        [Parent] TaskEntity userEntity,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
    
    [BindMember(nameof(TaskEntity.TasksCreatedByUser))]
    public static async Task<IEnumerable<UserEntity>> GetTasksCreatedByUserAsync(
        [Parent] UserEntity userEntity,
        IUsersCreatedTasksDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
}