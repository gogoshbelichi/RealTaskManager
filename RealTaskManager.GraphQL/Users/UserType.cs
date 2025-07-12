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
            .ResolveNode(
                async (ctx, id)
                    => await ctx.DataLoader<IUserByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted))
            .UseFiltering<UserFilterInputType>();

        descriptor.Field("email").Resolve(ctx =>
        {
            var user = ctx.GetUser() ?? throw new UserNotFoundException();
            return user.FindFirstValue(ClaimTypes.Email) ?? throw new UserNotFoundException();
        });

        descriptor.Field("roles").Resolve(ctx =>
        {
            var user = ctx.GetUser() ?? throw new UserNotFoundException();
            // FindAll никогда не вернёт null, только пустой список.
            return user.FindAll(ClaimTypes.Role).Select(r => r.Value);
        });

        descriptor.Field("username").Resolve(ctx =>
        {
            var user = ctx.GetUser() ?? throw new UserNotFoundException();
            return user.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername) ?? throw new UserNotFoundException();
        });
    }
    
    [BindMember(nameof(UserEntity.TasksCreatedByUser))]
    [GraphQLName("tasksCreated")]
    public static async Task<IEnumerable<TaskEntity>> GetTasksCreatedByUserAsync(
        [Parent] UserEntity userEntity,
        ITasksCreatedByUserDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
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
        return await loader
            .Select(selection)
            .LoadRequiredAsync(user.Id, ct);
    }
}