using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;

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

        /*descriptor.Field(t => t.TasksCreated)
            .UsePaging()
            .UseFiltering<TasksCreatedByUserFilterInputType>();*/

        descriptor.Field(t => t.TasksAssignedToUser)
            .UsePaging()
            .UseFiltering<TasksAssignedToUserFilterInputType>();
    }
    
    //[BindMember(nameof(UserEntity.TasksCreatedByUser))]
    [GraphQLName("tasksCreated")]
    public static async Task<IEnumerable<TaskEntity>> GetTasksCreatedAsync(
        [Parent] UserEntity userEntity,
        ITasksCreatedByUserDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("UserType GetTasksCreatedByUserAsync");
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }
    
    [BindMember(nameof(UserEntity.TasksAssignedToUser))]
    [GraphQLName("tasksAssigned")]
    public static async Task<IEnumerable<TaskEntity>> GetTasksAssignedToUserAsync(
        [Parent] UserEntity user,
        ITasksAssignedToUserDataLoader loader,
        ISelection selection,
        CancellationToken ct)
    {
        Console.WriteLine("UserType GetTasksAssignedToUserAsync");
        return await loader
            .Select(selection)
            .LoadRequiredAsync(user.Id, ct);
    }
}