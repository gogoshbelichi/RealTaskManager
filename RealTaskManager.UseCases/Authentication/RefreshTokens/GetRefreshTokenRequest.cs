namespace RealTaskManager.UseCases.Authentication.RefreshTokens;

public sealed class GetRefreshTokenRequest
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    
}