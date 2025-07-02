namespace RealTaskManager.Core.Entities;

// this is not identity user it's a task manager user for graphQL maybe I will change as far as I reach new conditions
public class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string IdentityId { get; private set; } = string.Empty;
    
    public ICollection<TaskEntity> CreatedTasks { get; init; } = [];
    public ICollection<TaskEntity> AssignedTasks { get; init; } = [];

    public UserEntity(string email, string username, string identityId)
    {
        Email = email;
        Username = username;
        IdentityId = identityId;
        CreatedTasks = [];
        AssignedTasks = [];
    }

    private UserEntity(){}
}