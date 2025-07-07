using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Users;

[ObjectType<UserEntity>]
public static partial class UserType
{ 
    static partial void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.Authorize(new[] { "User", "Administrator" });
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(
                async (ctx, id)
                    => await ctx.DataLoader<IUserByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }
    
    [BindMember(nameof(UserEntity.TasksAssignedToUser))]
    public static async Task<IEnumerable<TaskEntity>> GetTasksAssignedToUserAsync(
        [Parent] UserEntity userEntity,
        ITasksAssignedToUserIdDataLoader assignedToUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await assignedToUserId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
    
    [BindMember(nameof(UserEntity.TasksCreatedByUser))]
    public static async Task<IEnumerable<TaskEntity>> GetTasksCreatedByUserAsync(
        [Parent] UserEntity userEntity,
        ITasksCreatedByUserIdDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
}