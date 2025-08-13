using System.ComponentModel.DataAnnotations;

namespace RealTaskManager.Core.Entities;

public sealed class UserEntity
{
    public Guid Id { get; init; }
    [MaxLength(254)] //RFC 5321 4.5.3.1 (254 to fit smtp)
    public required string Email { get; init; }
    [MaxLength(32)]
    public string? Username { get; init; }
    public ICollection<string> Roles { get; init; } = [];
    [MaxLength(36)]
    public ICollection<TasksAssignedToUser> TasksAssignedToUser { get; init; } = [];
    public ICollection<TaskEntity> TasksCreated { get; init; } = [];
}