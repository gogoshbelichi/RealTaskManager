using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;
using RealTaskManager.GraphQL.Users;

namespace RealTaskManager.GraphQL.TasksAssignedToUsers;


public class TasksAssignedToUserType : ObjectType<TasksAssignedToUser>
{
    protected override void Configure(IObjectTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(t => t.Task);
        descriptor.Field(t => t.User);
        
        descriptor.Ignore(t => t.UserId).Authorize("AdminPolicy");
        descriptor.Ignore(t => t.TaskId).Authorize("UserPolicy");
    }
}