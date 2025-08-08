using HotChocolate.Data.Sorting;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TasksSorting : SortInputType<TaskEntity> 
{
    protected override void Configure(ISortInputTypeDescriptor<TaskEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(t => t.Title);
        descriptor.Field(t => t.Description);
        descriptor.Field(t => t.Status);
        descriptor.Field(t => t.CreatedAt);
    }
}