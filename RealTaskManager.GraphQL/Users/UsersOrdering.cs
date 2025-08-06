using GreenDonut.Data;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Users;

public static class UsersOrdering
{
    public static SortDefinition<UserEntity> UsersDefaultOrder(SortDefinition<UserEntity> sort) 
        => sort.IfEmpty(sd 
            => sd.AddAscending(u => u.Username)
                .AddAscending(u => u.Id));
}