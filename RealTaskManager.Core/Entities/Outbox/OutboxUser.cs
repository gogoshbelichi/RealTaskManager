namespace RealTaskManager.Core.Entities.Outbox;
// I thought I needed it before for transactional outbox pattern
// because of 2 dbContexts(but it seems antipattern in scope of one service)
// for UserProfiles and AspNetUsers
// but for now it is obsolete, btw we can use it
// when we will have to fetch with some-kind outside user service and continue developing
// this feature in future
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