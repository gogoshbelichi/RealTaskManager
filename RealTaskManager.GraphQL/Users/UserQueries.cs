using System.Security.Claims;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Users;

[QueryType]
public static class UserQueries
{
    [Authorize("AdminPolicy")]
    [UsePaging]
    [UseFiltering(typeof(UserFilterInputType))]
    [UseSorting]
    public static IQueryable<UserEntity> GetUsers(RealTaskManagerDbContext dbContext)
    {
        Console.WriteLine("UserQueries GetUsers");
        return dbContext.Users.AsNoTracking().OrderBy(t => t.Id);
    }
    
    [Authorize("AdminPolicy")]
    [NodeResolver]
    public static async Task<UserEntity?> GetUserByIdAsync(
        Guid id,
        IUserByIdDataLoader userById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("UserQueries GetUserByIdAsync");
        return await userById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    [Authorize("AdminPolicy")]
    public static async Task<IEnumerable<UserEntity>> GetUsersByIdAsync(
        [ID<UserEntity>] Guid[] ids,
        IUserByIdDataLoader userById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("UserQueries GetUsersByIdAsync");
        return await userById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
    
    [Authorize("UserPolicy")]
    public static IQueryable<UserEntity> GetMe(
        RealTaskManagerDbContext dbContext,
        ClaimsPrincipal? principal)
    {
        Console.WriteLine("UserQueries GetMe");
        var userId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) throw new UserNotFoundException();
        return dbContext.Users.AsNoTracking().Where(u => u.IdentityId == userId)
            .Include(u => u.TasksAssignedToUser)
            .ThenInclude(t => t.Task)
            .Include(u => u.TasksCreatedByUser)
            .ThenInclude(t => t.Task);
    }
}