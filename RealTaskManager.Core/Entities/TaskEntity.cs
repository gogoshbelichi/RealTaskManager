namespace RealTaskManager.Core.Entities;

public class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public UserEntity CreatedBy { get; set; } = null!;
    public Guid AssignedToId { get; set; }
    public UserEntity? AssignedTo { get; set; }
}