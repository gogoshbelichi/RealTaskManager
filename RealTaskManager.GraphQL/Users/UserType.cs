using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AssignedTasks;

namespace RealTaskManager.GraphQL.Users;

[ObjectType<UserEntity>]
public static partial class UserType
{ 
    static partial void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<IUserByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
    }
    
    [UsePaging]
    [BindMember(nameof(UserEntity.TasksCreated))]
    [GraphQLName("tasksCreatedlol")]
    public static async Task<Connection<TaskEntity>> GetTasksCreatedAsync(
        [Parent(requires: nameof(UserEntity.TasksCreated))] UserEntity userEntity,
        ITasksCreatedByUserDataLoader tasksCreatedByUserId,
        ISelection selection,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .With(args)
            .Select(selection)
            .LoadAsync(userEntity.Id, cancellationToken)
            .ToConnectionAsync();
    }
    
    [UsePaging]
    [BindMember(nameof(UserEntity.TasksAssignedToUser))]
    [GraphQLName("tasksAssigned")]
    [GraphQLType<ListType<NonNullType<TasksAssignedToUserType>>>]
    public static async Task<Connection<TasksAssignedToUser>> GetTasksAssignedToUserAsync(
        [Parent(requires: nameof(UserEntity.TasksAssignedToUser))] UserEntity user,
        ITasksAssignedToUsersDataLoader taskAssignedToUserId,
        QueryContext<TasksAssignedToUser> query,
        PagingArguments args,
        CancellationToken ct)
    {
        return await taskAssignedToUserId
            .With(args, query)
            .LoadAsync(user.Id, ct)
            .ToConnectionAsync();
    }
}