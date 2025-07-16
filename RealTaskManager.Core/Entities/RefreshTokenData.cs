using System.ComponentModel.DataAnnotations.Schema;

namespace RealTaskManager.Core.Entities;

public sealed class RefreshTokenData()
{
    public Guid Id { get; set; }
    public required string Token { get; init; }
    public required string Jti { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsUsed { get; private set; } = false;
    public bool IsRevoked { get; private set; } = false;
    public required TaskManagerUser User { get; init; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; private set; }
    
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    public void UseRefreshToken(RefreshTokenData data)
    {
        data.IsUsed = true;
    }
    
    public bool RevokeRefreshToken(RefreshTokenData data)
    {
        data.IsRevoked = true;
        data.RevokedAt = DateTimeOffset.UtcNow;
        return data.IsRevoked;
    }
}