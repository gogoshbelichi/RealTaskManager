using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Users;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TaskFilter : FilterInputType<TaskEntity>
{
    protected override void Configure(IFilterInputTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(s => s.Status);

        descriptor.Field(s => s.CreatedBy);
        
        descriptor.Field(s => s.TasksAssignedToUser);
    }
}