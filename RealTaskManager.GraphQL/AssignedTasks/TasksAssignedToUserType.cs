using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.AssignedTasks;

public class TasksAssignedToUserType : ObjectType<TasksAssignedToUser>
{
    protected override void Configure(IObjectTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("tasksAss");
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(s => s.TaskId)
            .ID<TaskEntity>();
        descriptor.Field(s => s.Task);
    }
}