using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Users;

[ObjectType<UserEntity>]
public static partial class UserType
{ 
    static partial void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        //descriptor.Authorize("User", "Administrator");
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<IUserByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
        
        descriptor.Field(u => u.Username);
        
        descriptor.Field(u => u.Email);

        descriptor.Field(u => u.Roles);
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
        Console.WriteLine("UserType GetTasksCreatedByUserAsync");
        return await tasksCreatedByUserId
            .With(args)
            .Select(selection)
            .LoadAsync(userEntity.Id, cancellationToken).ToConnectionAsync();
    }
    
    [UsePaging]
    [BindMember(nameof(UserEntity.TasksAssignedToUser))]
    [GraphQLName("tasksAssigned")]
    public static async Task<Connection<TasksAssignedToUser>> GetTasksAssignedToUserAsync(
        [Parent(requires: nameof(UserEntity.TasksAssignedToUser))] UserEntity user,
        ITasksAssignedToUserDataLoader loader,
        ISelection selection,
        PagingArguments args,
        CancellationToken ct)
    {
        Console.WriteLine("UserType GetTasksAssignedToUserAsync");
        return await loader
            .With(args)
            .Select(selection)
            .LoadAsync(user.Id, ct).ToConnectionAsync();
    }
}