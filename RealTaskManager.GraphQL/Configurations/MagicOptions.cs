namespace RealTaskManager.GraphQL.Configurations;

public sealed class MagicOptions
{
    public required string MagicString { get; init; }
    public required string MagicAudience { get; init; }
    public required string MagicIssuer { get; init; }
    public required int MagicDaysForRefresh { get; init; }
    public required int MagicMinutesToAccess { get; init; }
}