using System.ComponentModel.DataAnnotations;

namespace RealTaskManager.Core.Entities;

// this is not identity user it's a task manager user for graphQL maybe I will change as far as I reach new conditions
public sealed class UserEntity
{
    public Guid Id { get; private set; }
    [MaxLength(36)]
    public required string IdentityId { get; init; }
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TasksCreatedByUser> TasksCreatedByUser { get; init; } = [];
}