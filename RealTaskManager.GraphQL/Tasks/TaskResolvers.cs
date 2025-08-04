using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TaskResolvers
{
    public IQueryable<TasksAssignedToUser> GetTasksAssignedToUserObjects(
        [Parent] TaskEntity taskEntity,
        RealTaskManagerDbContext dbContext)
    {
        Console.WriteLine("TaskResolvers GetTasksAssignedToUserObjects");
        return dbContext.TasksAssignedToUsers
            .Where(tcu => tcu.TaskId == taskEntity.Id)
            .Include(tcu => tcu.User);
    }
    
    public IQueryable<TasksAssignedToUser> GetTasksAssignedToUserObjectsForUser(
        [Parent] UserEntity userEntity,
        RealTaskManagerDbContext dbContext)
    {
        Console.WriteLine("TaskResolvers GetTasksAssignedToUserObjectsForUser");
        return dbContext.TasksAssignedToUsers
            .Where(tcu => tcu.UserId == userEntity.Id)
            .Include(tcu => tcu.Task);
    }
}