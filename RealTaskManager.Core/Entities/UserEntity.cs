using System.ComponentModel.DataAnnotations;

namespace RealTaskManager.Core.Entities;

// this is not identity user it's a task manager user for graphQL maybe I will change as far as I reach new conditions
public sealed class UserEntity
{
    public Guid Id { get; init; }
    [MaxLength(254)] //RFC 5321 4.5.3.1 (254 to fit smtp)
    public required string Email { get; init; }
    [MaxLength(32)]
    public string? Username { get; init; }
    public ICollection<string> Roles { get; init; } = [];
    [MaxLength(36)]
    public required string IdentityId { get; init; }
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
    
    public int NumberAssingnedTasks => TasksCreatedByUser.Count(tu => tu.UserId == Id);
    
    public int NumberCreatedTasks => TasksCreatedByUser.Count(tu => tu.UserId == Id);
    
    //public ICollection<TaskEntity> CretedTasks { get; init; } = []; //easier to do like this but what if we have a co-authors
}