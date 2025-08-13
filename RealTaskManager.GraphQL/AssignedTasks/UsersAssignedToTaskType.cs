using Microsoft.AspNetCore.Mvc.ModelBinding;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.AssignedTasks;

public class UsersAssignedToTaskType : ObjectType<TasksAssignedToUser>
{
    protected override void Configure(IObjectTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("assignedT");
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(s => s.UserId)
            .ID<UserEntity>();
        descriptor.Field(s => s.User);
    }
}