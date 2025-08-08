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
        //descriptor.Authorize("User", "Administrator");
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(async (ctx, id)
                => await ctx.DataLoader<ITaskByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));
        
        descriptor
            .Field(s => s.CreatedByUserId)
            .ID<UserEntity>();
    }
    
    [UsePaging]
    [BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    [GraphQLName("assignedTo")]
    public static async Task<Connection<TasksAssignedToUser>> GetUsersAssignedToTasksAsync(
        [Parent(requires: nameof(TaskEntity.TasksAssignedToUser))] TaskEntity task,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        ISelection selection,
        PagingArguments args,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .With(args)
            .Select(selection)
            .LoadAsync(task.Id, cancellationToken)
            .ToConnectionAsync();
    }
}