using HotChocolate.Authorization;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public record AddTaskInput(
    string Title,
    string? Description,
    TaskStatusEnum? Status,
    [property: Authorize("AdminPolicy")] [property: ID<UserEntity>] string? AssignToUserByUsername
);

public record AssignTaskInput(
    [property: ID<UserEntity>] IReadOnlyList<Guid> AssignedToUserId
);

public record UpdateTaskInput(
    [property: ID<TaskEntity>] Guid Id,
    string? Title,
    string? Description,
    TaskStatusEnum? Status
);

public record TakeTaskInput([property: ID<TaskEntity>] Guid Id );

public record DeleteTaskInput([property: ID<TaskEntity>] Guid Id);