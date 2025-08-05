using System.Security.Claims;
using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL.Users;

[QueryType]
public static class UserQueries
{
    //[Authorize("AdminPolicy")]
    [UsePaging]
    [UseFiltering(typeof(UserFilterInputType))]
    [UseSorting]
    public static IQueryable<UserEntity> GetUsers(RealTaskManagerDbContext dbContext)
    {
        Console.WriteLine("UserQueries GetUsers");
        return dbContext.UserProfiles.AsNoTracking().OrderBy(u => u.Id);
    }
    
    //[Authorize("AdminPolicy")]
    [NodeResolver]
    public static async Task<UserEntity?> GetUserByIdAsync(
        Guid id,
        IUserByIdDataLoader userById,
        ISelection selection,
        CancellationToken ct)
    {
        Console.WriteLine("UserQueries GetUserByIdAsync");
        return await userById.Select(selection).LoadAsync(id, ct);
    }
    
    //[Authorize("AdminPolicy")]
    [UsePaging]
    public static async Task<Connection<UserEntity>> GetUsersByIdAsync(
        [ID<UserEntity>] Guid[] ids,
        IPagedUsersByIdDataLoader usersById,
        ISelection selection,
        PagingArguments args,
        CancellationToken ct)
    {
        Console.WriteLine("UserQueries GetUsersByIdAsync");
        return await usersById
            .With(args)
            .Select(selection)
            .LoadRequiredAsync(ids, ct).ToConnectionAsync();
    }
    
    
    
    [GraphQLDescription("You, your created tasks and tasks you are assigned to")]
    //[Authorize("UserPolicy")]
    public static IQueryable<UserEntity> GetMe(
        ISelection selection,
        RealTaskManagerDbContext dbContext,
        ClaimsPrincipal? principal)
    {
        Console.WriteLine("UserQueries GetMe");
        var username = principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.PreferredUsername)?.Value;
        if (username is null) throw new UserNotFoundException();
        return dbContext.UserProfiles.AsNoTracking().Where(u => u.Username == username)
            .Select(selection);
    }
}