using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

public sealed class TaskResolvers
{
    public IQueryable<UserEntity> GetUsersCreatedTasks(
        [Parent] TaskEntity taskEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksCreatedByUsers.Where(tcu => tcu.TaskId == taskEntity.Id)
            .Select(tcu => tcu.User);
    }
    
    public IQueryable<UserEntity> GetUsersAssignedToTasks(
        [Parent] TaskEntity taskEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksAssignedToUsers.Where(tcu => tcu.TaskId == taskEntity.Id)
            .Select(tcu => tcu.User);
    }
    
    public IQueryable<TaskEntity> GetTasksCreatedByUsers(
        [Parent] UserEntity userEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksCreatedByUsers.Where(tcu => tcu.UserId == userEntity.Id)
            .Select(tcu => tcu.Task);
    }
    
    public IQueryable<TaskEntity> GetTasksAssignedToUsers(
        [Parent] UserEntity userEntity,
        RealTaskManagerDbContext dbContext)
    {
        return dbContext.TasksAssignedToUsers.Where(tcu => tcu.UserId == userEntity.Id)
            .Select(tcu => tcu.Task);
    }
}