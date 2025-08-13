namespace RealTaskManager.Core.Entities;
// decided to make many-to-many,
// for example in future I want to have subtasks and some assignee have to take one subtask
// and some other assignee can take some other subtask
public sealed class TasksAssignedToUser
{
    public Guid TaskId { get; init; }
    
    public TaskEntity Task { get; init; } = null!;
    
    public Guid UserId { get; init; }

    public UserEntity User { get; init; } = null!;
    
    public required DateTimeOffset LastAssignedAt { get; set; }
}