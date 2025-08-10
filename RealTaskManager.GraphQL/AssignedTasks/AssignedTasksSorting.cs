using HotChocolate.Data.Sorting;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.AssignedTasks;
// need to develop, implement and test later
public sealed class AssignedTasksSorting : SortInputType<TasksAssignedToUser> 
{
    protected override void Configure(ISortInputTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.User);
        descriptor.Field(t => t.UserId);
    }
}