using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Users;

namespace RealTaskManager.GraphQL.Tasks;

public class TaskType : ObjectType<TaskEntity>
{
    protected override void Configure(IObjectTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        //descriptor.Authorize("User", "Administrator");
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<ITaskByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
        
        descriptor.Field(s => s.CreatedByUserId)
            .ID<UserEntity>();
        
        descriptor.Field(t => t.Title);
        descriptor.Field(t => t.Description);
        descriptor.Field(t => t.Status);
        descriptor.Field(t => t.CreatedBy);
    }
    
    [GraphQLName("assignedTo")]
    public async Task<UserEntity[]> GetUserAssignedToTaskAsync(
        [Parent] TaskEntity userEntity,
        IUsersAssignedToTaskDataLoader userAssignedToTaskId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
    
    [BindField(nameof(TaskEntity.CreatedBy))]
    [GraphQLName("createdBy")]
    public async Task<UserEntity> GetUserCreatedTaskAsync(
        [Parent(requires: nameof(TaskEntity.CreatedBy))] TaskEntity taskEntity,
        IUserCreatedTaskDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(taskEntity.Id, cancellationToken);
    }
}