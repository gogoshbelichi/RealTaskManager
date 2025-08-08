using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TaskFilterInputType : FilterInputType<TaskEntity>
{
    protected override void Configure(IFilterInputTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(s => s.Status);

        descriptor.Field(s => s.CreatedBy);
        
        descriptor.Field(s => s.TasksAssignedToUser);
    }
}

public sealed class TasksAssignedToUserFilterInputType : FilterInputType<TasksAssignedToUser>
{
    protected override void Configure(IFilterInputTypeDescriptor<TasksAssignedToUser> descriptor)
    {
        descriptor.Name("TasksAssignedToUserFilter");
        descriptor.BindFieldsExplicitly();

        descriptor.Field(t => t.User);
    }
}