using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.TasksAssignedToUsers;

public sealed class UsersAssignedToTasksFilter : FilterInputType<TasksAssignedToUser>
{
    protected override void Configure(IFilterInputTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("CustomUsersAssignedToTasksFilter");
        descriptor.BindFieldsExplicitly();

        descriptor.Field(t => t.Task);
    }
}