namespace RealTaskManager.UseCases.Authentication.RefreshTokens;

public sealed record GetRefreshTokenRequest
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    
}