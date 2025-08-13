using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public record AddTaskInput(
    string Title,
    string? Description,
    TaskStatusEnum? Status
);

public record AssignTaskInput(
    [property: ID<UserEntity>] IReadOnlyList<Guid> AssignedToUserId
);

public record UpdateTaskDetailsInput(
    [property: ID<TaskEntity>] Guid TaskId,
    string? Title,
    string? Description,
    TaskStatusEnum? Status
);

public record TakeTaskInput([property: ID<TaskEntity>] Guid Id );

public record UpdateTaskAssignmentInput(
    [property: ID<TaskEntity>] Guid TaskId,
    [property: ID<UserEntity>] Guid[] AssignToUserIds,
    [property: ID<UserEntity>] Guid[] RemoveAssignmentToUserIds
);

public record DeleteTaskInput([property: ID<TaskEntity>] Guid Id);