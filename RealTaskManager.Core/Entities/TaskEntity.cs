namespace RealTaskManager.Core.Entities;

public sealed class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Todo";
    public DateTime CreatedAt { get; set; }
    
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
}