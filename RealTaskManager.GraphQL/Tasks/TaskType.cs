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
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<ITaskByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));

        descriptor.Field(t => t.TasksCreatedByUser)
            .ResolveWith<TaskResolvers>(r => r.GetUsersCreatedTasks(default!, default!))
            .UseFiltering<TasksCreatedByUserFilterInputType>();
        
        descriptor.Field(t => t.TasksAssignedToUser)
            .ResolveWith<TaskResolvers>(r => r.GetUsersAssignedToTasks(default!, default!))
            .UseFiltering<TasksAssignedToUserFilterInputType>();
    }
    
    [BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    [GraphQLName("assignedTo")]
    public static async Task<IEnumerable<UserEntity>> GetUsersAssignedToTasksAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity userEntity,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
    
    [BindMember(nameof(TaskEntity.TasksCreatedByUser))]
    [GraphQLName("createdBy")]
    public static async Task<IEnumerable<UserEntity>> GetUsersCreatedTaskAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity taskEntity,
        IUsersCreatedTasksDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(taskEntity.Id, cancellationToken);
    }
}