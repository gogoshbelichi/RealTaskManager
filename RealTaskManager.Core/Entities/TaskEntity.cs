using System.ComponentModel.DataAnnotations;

namespace RealTaskManager.Core.Entities;

public sealed class TaskEntity
{
    public Guid Id { get; set; }
    [MaxLength(200)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public TaskStatusEnum Status { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; set; } = [];
    public ICollection<UserEntity> UsersAssigned { get; set; } = [];
    public required Guid CreatedByUserId { get; init; }
    public required UserEntity CreatedBy { get; init; }
}

