using GreenDonut.Data;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.AssignedTasks;
// need to develop, implement and test later
public static class AssignedTasksOrdering
{
    public static SortDefinition<TasksAssignedToUser> UsersAssignedDefaultOrder(
        SortDefinition<TasksAssignedToUser> sort)
        => sort.IfEmpty(sd
            => sd.AddAscending(ta => ta.UserId));
    
    public static SortDefinition<TasksAssignedToUser> TasksAssignedDefaultOrder(
        SortDefinition<TasksAssignedToUser> sort)
        => sort.IfEmpty(sd
            => sd.AddAscending(ta => ta.TaskId));
}