namespace RealTaskManager.GraphQL.AuthEndpoints;

/// <summary>
///  Tokens payload
/// </summary>
public record TokensResponse(string access_token, string refresh_token);