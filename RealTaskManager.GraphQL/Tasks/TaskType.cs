using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AssignedTasks;

namespace RealTaskManager.GraphQL.Tasks;

[ObjectType<TaskEntity>]
public static partial class TaskType
{
    static partial void Configure(IObjectTypeDescriptor<TaskEntity> descriptor)
    {
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
    [GraphQLType<ListType<NonNullType<UsersAssignedToTaskType>>>]
    public static async Task<Connection<TasksAssignedToUser>> GetUsersAssignedToTasksAsync(
        [Parent(requires: nameof(TaskEntity.TasksAssignedToUser))] TaskEntity task,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        QueryContext<TasksAssignedToUser> query,
        PagingArguments args,
        CancellationToken ct)
    {
        return await userAssignedToTaskId
            .With(args, query)
            .LoadAsync(task.Id, ct)
            .ToConnectionAsync();
    }
}