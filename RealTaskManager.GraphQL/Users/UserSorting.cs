using HotChocolate.Data.Sorting;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Users;

public sealed class UserSorting : SortInputType<UserEntity> 
{
    protected override void Configure(ISortInputTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Username);
        descriptor.Field(x => x.Email);
    }
}