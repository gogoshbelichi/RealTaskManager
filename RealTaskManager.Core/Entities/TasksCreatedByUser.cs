namespace RealTaskManager.Core.Entities;

public sealed class TasksCreatedByUser
{
    public Guid TaskId { get; init; }

    public TaskEntity Task { get; init; } = null!;

    public Guid UserId { get; init; }

    public UserEntity User { get; init; } = null!;
}