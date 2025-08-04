using HotChocolate.Data.Filters;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Tasks;

namespace RealTaskManager.GraphQL.Users;

public sealed class UserFilterInputType : FilterInputType<UserEntity>
{
    protected override void Configure(IFilterInputTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(u => u.Email);
        descriptor.Field(u => u.Username);
        descriptor.Field(u => u.Roles);
    }
}

/*public sealed class UsersCreatedTasksFilterInputType : FilterInputType<TasksCreatedByUser>
{
    protected override void Configure(IFilterInputTypeDescriptor<TasksCreatedByUser> descriptor)
    {
        descriptor.Name("UsersCreatedTasksFilter");
        descriptor.BindFieldsExplicitly();

        descriptor.Field(t => t.Task).Type<TaskFilterInputType>();
    }
}*/