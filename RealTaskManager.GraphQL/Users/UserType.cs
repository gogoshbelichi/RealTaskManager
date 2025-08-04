using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.TasksAssignedToUsers;

namespace RealTaskManager.GraphQL.Users;

public class UserType : ObjectType<UserEntity>
{ 
    protected override void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        //descriptor.Authorize("User", "Administrator");
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<IUserByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
        
        descriptor.Field(u => u.Username).Type<StringType>();
        
        descriptor.Field(u => u.Email).Type<StringType>();

        descriptor.Field(u => u.Roles);
    }
    
    [BindField(nameof(UserEntity.TasksCreated))]
    [GraphQLName("tasksCreated")]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<TaskEntity>> GetTasksCreatedAsync(
        [Parent] UserEntity userEntity,
        ITasksCreatedByUserDataLoader tasksCreatedByUserId,
        ISelection selection,
        PagingArguments args,
        CancellationToken ct)
    {
        Console.WriteLine("UserType GetTasksCreatedByUserAsync");
        return await tasksCreatedByUserId
            .With(args)
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, ct)
            .ToConnectionAsync();
    }
    
    [BindField(nameof(UserEntity.TasksAssignedToUser))]
    [GraphQLName("tasksAssignedToUser")]
    [UsePaging]
    [UseFiltering(typeof(UsersAssignedToTasksFilter))]
    [UseSorting]
    public static async Task<Connection<TasksAssignedToUser>> GetTasksAssignedToUserIdAsync(
        [Parent] UserEntity user,
        ITasksAssignedToUserByUserIdDataLoader tasksAssignedToUserByUserId,
        ISelection selection,
        PagingArguments args,
        CancellationToken ct)
    {
        Console.WriteLine("UserType GetTasksAssignedToUserAsync");
        return await tasksAssignedToUserByUserId
            .With(args)
            .Select(selection)
            .LoadRequiredAsync(user.Id, ct)
            .ToConnectionAsync();
    }
}