namespace RealTaskManager.Core.Entities;

// this is not identity user it's a task manager user for graphQL maybe I will change as far as I reach new conditions
public sealed class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty; 
    public string IdentityId { get; init; } = string.Empty;
    
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
}