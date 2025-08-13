using System.Collections.Immutable;
using System.Security.Claims;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Tasks;

[MutationType]
public static class TasksMutations
{
    [Authorize("UserPolicy")]
    [Error<TitleEmptyException>, Error<TaskNotCreatedError>] // later could improve exceptions and it's payloads
    public static async Task<FieldResult<TaskEntity, TaskNotCreatedError, UserNotFoundError>> AddTaskAsync(
        AddTaskInput input,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct) // works
    {
        if (string.IsNullOrEmpty(input.Title)) 
            throw new TitleEmptyException();
        
        var username = claims.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);
        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == username, ct);
        
        if (user is null)
            return new UserNotFoundError();
        
        var result = await AddTaskTransaction(input, dbContext, ct, user);

        return result;
    }

    // make it clean and move somewhere later
    private static async Task<FieldResult<TaskEntity, TaskNotCreatedError, UserNotFoundError>> AddTaskTransaction(
        AddTaskInput input,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct, 
        UserEntity user) // works
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var task = new TaskEntity
            {
                Title = input.Title,
                Description = input.Description,
                Status = input.Status ?? TaskStatusEnum.Backlog,
                CreatedBy = user,
                CreatedByUserId = user.Id,
            };
        
            await dbContext.Tasks.AddAsync(task, ct);
            
            await dbContext.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            return task;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            // ... retries if needed, but I'll return as it isw 
            return new TaskNotCreatedError();
        }
    }

    [Authorize("UserPolicy")]
    [Error<PermissionException>]
    [GraphQLDescription("To make changes in title, description or status")]
    public static async Task<FieldResult<TaskEntity, TaskNotFoundError, UserNotFoundError>> UpdateTaskDetailsAsync(
        UpdateTaskDetailsInput detailsInput,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct) // works
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == detailsInput.TaskId, ct);
        
        if (task is null)
            return new TaskNotFoundError();

        var username = claims.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);
        var user = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken: ct);
        
        if (user is null)
            return new UserNotFoundError();
        
        // u can change details if you're an admin or creator
        // better to make some permission (resource-based) authorization (refactor later)
        if (task.CreatedByUserId != user.Id && !claims.IsInRole("Administrator")) 
            throw new PermissionException();
        
        task.Title = detailsInput.Title ?? task.Title;
        task.Description = detailsInput.Description ?? task.Description;
        task.Status = detailsInput.Status ?? task.Status;
        
        await dbContext.SaveChangesAsync(ct);

        return task;
    }
    
    [Authorize("AdminPolicy")]
    [Error<PermissionException>]
    [GraphQLDescription("To make changes in assignment")]
    public static async Task<FieldResult<TaskEntity, TaskNotFoundError, UsersNotAssignedError>> UpdateTaskAssignment(
        UpdateTaskAssignmentInput input,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct) // works well
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var task = await dbContext.Tasks
                .Include(t => t.TasksAssignedToUser)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == input.TaskId, ct);
            
            if (task is null)
                return new TaskNotFoundError();
        
            if (!input.AssignByUserIds.IsEmpty())
                task = AssignUsersByUserIds(task, input);

            if (!input.UserIdsToRemove.IsEmpty())
                task = RemoveAssignmentByUserIds(task, input, dbContext);
        
            await dbContext.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            return task;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            return new UsersNotAssignedError();
        }
    }

    // must be a contract in app layer
    private static TaskEntity AssignUsersByUserIds(
        TaskEntity task,
        UpdateTaskAssignmentInput input)
    {
        var userIds = input.AssignByUserIds.ToImmutableHashSet();
        
        var existingUserIds = task.TasksAssignedToUser
            .Where(tau => userIds.Contains(tau.UserId))
            .Select(tau => tau.UserId);
        
        foreach (var performerId in userIds.Except(existingUserIds))
        {
            task.TasksAssignedToUser.Add(new TasksAssignedToUser()
            {
                Task = task,
                UserId = performerId,
                LastAssignedAt = DateTimeOffset.UtcNow
            });
        }
        
        return task;
    }

    // must be a contract in app layer
    private static TaskEntity RemoveAssignmentByUserIds(
        TaskEntity task,
        UpdateTaskAssignmentInput input, 
        RealTaskManagerDbContext dbContext)
    {
        var userIdsToRemove = input.UserIdsToRemove.ToImmutableHashSet(); 
        
        var formerPerformersIds = dbContext.UserProfiles
            .Where(u => userIdsToRemove.Contains(u.Id))
            .Select(entity => entity.Id);

        var formerAssignments = task.TasksAssignedToUser
            .Where(ta => formerPerformersIds.Contains(ta.UserId));
        
        dbContext.RemoveRange(formerAssignments);
        
        return task;
    }
    
    private static bool IsEmpty(this IEnumerable<Guid> ids) => !ids.Any();

    [Authorize("UserPolicy")]
    public static async Task<FieldResult<ResultTaskPayload, TaskNotFoundError, UserNotFoundError>> DeleteTaskAsync(
        DeleteTaskInput input,
        ClaimsPrincipal claims,
        RealTaskManagerDbContext dbContext,
        CancellationToken ct)
    {
        var task = await dbContext.Tasks.FindAsync([input.Id], ct);

        if (task is null)
        {
            return new TaskNotFoundError();
        }
        
        var username = claims.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);

        var user = await dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Username == username, ct);
        
        if (user is null) 
            return new UserNotFoundError();

        if (claims.IsInRole("Administrator") || task.CreatedByUserId == user.Id)
        {
            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync(ct);

            return new ResultTaskPayload("The task has been deleted");
        }
        
        return new ResultTaskPayload("The task is not deleted");
    }
    
    // mb made it cope with multiple tasks later when user select more than one task to do
    [Authorize("UserPolicy")]
    public static async Task<FieldResult<TaskEntity, TaskNotFoundError, UserNotFoundError, TaskAlreadyAssignedError>> TakeTaskAsync(
        TakeTaskInput input,
        RealTaskManagerDbContext dbContext,
        ClaimsPrincipal claims,
        CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var task = await dbContext.Tasks.FindAsync([input.Id], ct);
            
            if (task is null) 
                return new TaskNotFoundError();

            var username = claims.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);

            var userId = await dbContext.UserProfiles
                .Where(u => u.Username == username)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(ct);
            
            if (userId == Guid.Empty)
                return new UserNotFoundError();
            
            // check assignment existence
            if (dbContext.TasksAssignedToUsers.Any(ta => ta.TaskId == input.Id && ta.UserId == userId))
            {
                return new TaskAlreadyAssignedError();
            }
            
            var assignment = new TasksAssignedToUser
            {
                Task = task,
                UserId = userId,
                LastAssignedAt = DateTimeOffset.UtcNow
            };
            
            await dbContext.TasksAssignedToUsers.AddAsync(assignment, ct);
            
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return task;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }                   
}