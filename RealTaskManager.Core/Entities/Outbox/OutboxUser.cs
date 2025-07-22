namespace RealTaskManager.Core.Entities.Outbox;

public sealed class OutboxUser
{
    public Guid Id { get; init; }
    public required UserEntity Content { get; init; }
    public UpdateType UpdateType { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public DateTime ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
}

public enum UpdateType
{
    Create,
    Read,
    Update,
    Delete
}