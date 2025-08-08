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
        
        /*descriptor.Field(t => t.TasksCreatedByUser)
            .ResolveWith<TaskResolvers>(r => 
                r.GetTasksCreatedByUserObjects(default!, default!))
            .UseFiltering<TasksCreatedByUserFilterInputType>();*/
    
        descriptor.Field(t => t.TasksAssignedToUser)
            .ResolveWith<TaskResolvers>(r => 
                r.GetTasksAssignedToUserObjects(default!, default!))
            .UseFiltering<TasksAssignedToUserFilterInputType>();
    }
    
    /*[BindMember(nameof(TaskEntity.TasksAssignedToUser))]
    [GraphQLName("assignedTo")]
    public static async Task<IEnumerable<UserEntity>> GetUsersAssignedToTasksAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity userEntity,
        IUsersAssignedToTasksDataLoader userAssignedToTaskId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userAssignedToTaskId
            .Select(selection)
            .LoadRequiredAsync(userEntity.Id, cancellationToken);
    }*/
    
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
    //
    /*[BindMember(nameof(TaskEntity.TasksCreatedByUser))]
    [GraphQLName("createdBy")]
    public static async Task<IEnumerable<UserEntity>> GetUsersCreatedTaskAsync(
        [Parent(nameof(TaskEntity.Id))] TaskEntity taskEntity,
        IUsersCreatedTasksDataLoader tasksCreatedByUserId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await tasksCreatedByUserId
            .Select(selection)
            .LoadRequiredAsync(taskEntity.Id, cancellationToken);
    }*/
    
    
}