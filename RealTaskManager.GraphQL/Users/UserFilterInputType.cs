using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;

namespace RealTaskManager.GraphQL.Users;

public sealed class UserFilterInputType : FilterInputType<UserEntity>
{
    protected override void Configure(IFilterInputTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(u => u.Email);
        descriptor.Field(u => u.Username);
        descriptor.Field(u => u.Roles);
        descriptor.Field(u => u.TasksAssignedToUser)
            .Type<ListFilterInputType<UsersAssignedToTasksFilterInputType>>();
    }
}

public sealed class UsersAssignedToTasksFilterInputType : FilterInputType<TasksAssignedToUser>
{
    protected override void Configure(IFilterInputTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("UsersAssignedToTasksFilter");
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(t => t.Task).Type<TaskFilterInputType>();
    }
}
