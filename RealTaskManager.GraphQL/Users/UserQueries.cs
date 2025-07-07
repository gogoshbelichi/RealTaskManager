using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Users;

[QueryType]
public static class UserQueries
{
    [Authorize("AdminPolicy")]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<UserEntity> GetUsers(RealTaskManagerDbContext dbContext)
    {
        return dbContext.Users.AsNoTracking().OrderBy(t => t.Id).ThenBy(t => t.IdentityId);
    }
    
    [Authorize("AdminPolicy")]
    [NodeResolver]
    public static async Task<UserEntity?> GetUserByIdAsync(
        Guid id,
        IUserByIdDataLoader userById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userById.Select(selection).LoadAsync(id, cancellationToken);
    }
    
    [Authorize("AdminPolicy")]
    public static async Task<IEnumerable<UserEntity?>> GetUsersByIdAsync(
        [ID<UserEntity>] Guid[] ids,
        IUserByIdDataLoader userById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await userById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}