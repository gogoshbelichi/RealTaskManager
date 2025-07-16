using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.UseCases.Authentication.RefreshTokens;

public class AddRefreshTokenHandler(CustomIdentityDbContext dbContext)
{
    public async Task<bool> HandleAsync(RefreshTokenData refreshTokenData, CancellationToken ct)
    {
        await dbContext.RefreshTokens.AddAsync(refreshTokenData, ct);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}