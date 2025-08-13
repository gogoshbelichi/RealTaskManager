using GreenDonut.Data;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public static class TaskOrdering
{
    public static SortDefinition<TaskEntity> TasksDefaultOrder(SortDefinition<TaskEntity> sort) 
        => sort.IfEmpty(sd 
            => sd.AddAscending(u => u.Title)
                .AddAscending(u => u.Id));
}