namespace RealTaskManager.UseCases.Authentication.RefreshTokens;

public sealed record GetRefreshTokenRequest
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    
}