namespace RealTaskManager.Core.Entities;

public sealed class TasksAssignedToUser
{
    public Guid TaskId { get; set; }
    
    public TaskEntity Task { get; set; } = null!;
    
    public Guid UserId { get; set; }

    public UserEntity User { get; set; } = null!;
    
    public required DateTimeOffset LastAssignedAt { get; set; }
}