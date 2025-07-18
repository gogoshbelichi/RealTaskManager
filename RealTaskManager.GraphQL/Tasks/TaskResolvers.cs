using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TaskResolvers
{
    public IQueryable<TasksCreatedByUser> GetTasksCreatedByUserObjects(
        [Parent] TaskEntity taskEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksCreatedByUsers
            .Where(tcu => tcu.TaskId == taskEntity.Id)
            .Include(tcu => tcu.User);
    }
    
    public IQueryable<TasksAssignedToUser> GetTasksAssignedToUserObjects(
        [Parent] TaskEntity taskEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksAssignedToUsers
            .Where(tcu => tcu.TaskId == taskEntity.Id)
            .Include(tcu => tcu.User);
    }
    
    public IQueryable<TasksCreatedByUser> GetTasksCreatedByUserObjectsForUser(
        [Parent] UserEntity userEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksCreatedByUsers
            .Where(tcu => tcu.UserId == userEntity.Id)
            .Include(tcu => tcu.Task);
    }
    
    public IQueryable<TasksAssignedToUser> GetTasksAssignedToUserObjectsForUser(
        [Parent] UserEntity userEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksAssignedToUsers
            .Where(tcu => tcu.UserId == userEntity.Id)
            .Include(tcu => tcu.Task);
    }
}