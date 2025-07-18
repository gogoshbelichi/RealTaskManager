using System.Security.Claims;
using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using Microsoft.IdentityModel.JsonWebTokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;
using JsonClaimValueTypes = System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes;

namespace RealTaskManager.GraphQL.Users;

[ObjectType<UserEntity>]
public static partial class UserType
{ 
    static partial void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.Authorize("User", "Administrator");
        descriptor.Field(u => u.IdentityId).Ignore();
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<IUserByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
        
        descriptor.Field(u => u.Username);
        
        descriptor.Field(u => u.Email);

        descriptor.Field(u => u.Roles);

        descriptor.Field(u => u.NumberCreatedTasks);
        
        descriptor.Field(u => u.NumberAssingnedTasks);

        descriptor.Field(t => t.TasksCreatedByUser)
            .ResolveWith<TaskResolvers>(r =>
                r.GetTasksCreatedByUserObjectsForUser(default!, default!))
            .UseFiltering<TasksCreatedByUserFilterInputType>();

        descriptor.Field(t => t.TasksAssignedToUser)
            .ResolveWith<TaskResolvers>(r =>
                r.GetTasksAssignedToUserObjectsForUser(default!, default!))
            .UseFiltering<TasksAssignedToUserFilterInputType>();
    }
    
    [BindMember(nameof(UserEntity.TasksCreatedByUser))]
    [GraphQLName("tasksCreated")]
    public static async Task<IEnumerable<TaskEntity>> GetTasksCreatedByUserAsync(
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