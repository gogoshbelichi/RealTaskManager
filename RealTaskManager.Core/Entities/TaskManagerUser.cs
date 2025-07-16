using Microsoft.AspNetCore.Identity;

namespace RealTaskManager.Core.Entities;

public class TaskManagerUser : IdentityUser
{
    public ICollection<RefreshTokenData> RefreshTokens { get; } = [];
}