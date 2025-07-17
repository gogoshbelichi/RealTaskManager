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
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
    
    //public UserEntity CreatedBy { get; init; } //easier to do like this but what if we have a co-authors
}

