using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.UseCases.Authentication.RefreshTokens;

public class GetRefreshTokenHandler(RealTaskManagerDbContext dbContext)
{
    public async Task<RefreshTokenData?> HandleAsync(GetRefreshTokenRequest request, CancellationToken ct)
    {
        var storedToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken, ct);
        
        return storedToken;
    }
}