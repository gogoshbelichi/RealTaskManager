using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.TasksAssignedToUsers;

[QueryType]
public static class TasksAssignedToUsersQueries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<TasksAssignedToUser> GetTasksAssignedToUsers(RealTaskManagerDbContext dbContext)
    {
        Console.WriteLine("TasksAssignedToUsersQueries GetTasksAssignedToUsers");
        return dbContext.TasksAssignedToUsers
            .AsNoTracking()
            .OrderBy(u => u.TaskId)
            .ThenBy(u => u.UserId);
    }
}