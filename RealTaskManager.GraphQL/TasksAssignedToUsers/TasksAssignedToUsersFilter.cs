using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.TasksAssignedToUsers;

public sealed class TasksAssignedToUsersFilter : FilterInputType<TasksAssignedToUser>
{
    protected override void Configure(IFilterInputTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("CustomAssignedToUserFilter");
        descriptor.BindFieldsExplicitly();

        descriptor.Field(t => t.User);
    }
}