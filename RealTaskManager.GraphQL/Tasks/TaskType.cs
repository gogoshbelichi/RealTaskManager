using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

[ObjectType<TaskEntity>]
public static partial class TaskType
{
    static partial void Configure(IObjectTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor.Authorize("User", "Administrator");
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(
                async (ctx, id)
                    => await ctx.DataLoader<ITaskByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }
    
    [UsePaging]
    [BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    public static async Task<Connection<UserEntity>> GetUsersAssignedToTasksAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity userEntity,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        PagingArguments pagingArguments,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .With(pagingArguments)
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken)
            .ToConnectionAsync();
    }
    
    [UsePaging]
    [BindMember(nameof(TaskEntity.TasksCreatedByUser))]
    public static async Task<Connection<UserEntity>> GetUsersCreatedTaskAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity taskEntity,
        IUsersCreatedTasksDataLoader tasksCreatedByUserId,
        ISelection selection,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .With(pagingArguments)
            .Select(selection)
            .LoadRequiredAsync(taskEntity.Id, cancellationToken)
            .ToConnectionAsync();
    }
}