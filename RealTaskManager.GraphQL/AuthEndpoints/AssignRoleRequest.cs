namespace RealTaskManager.GraphQL.AuthEndpoints;

/// <summary>
/// Assign Role Request form
/// </summary>
/// <param name="Role">Administrator or User...</param>
/// <param name="Email">Target user account by email for assignment</param>
public record AssignRoleRequest(string Role, string Email);