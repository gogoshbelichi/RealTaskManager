namespace RealTaskManager.Core.Entities;

public sealed class TaskEntity
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatusEnum Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
}

/*public class TaskStatusModel
{
    public Guid Id { get; init; }
    public string Status { get; protected set; }
    public ICollection<TaskEntity> Tasks { get; set; } = [];
}

public sealed class TaskStatusTodo : TaskStatusModel
{
    public TaskStatusTodo() => Status  = "To do";
} 

public sealed class TaskStatusInProgress : TaskStatusModel
{
    public TaskStatusInProgress() => Status = "In Progress";
} 

public sealed class TaskStatusDone : TaskStatusModel
{
    public TaskStatusDone() => Status = "Done";
} */

