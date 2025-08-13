namespace RealTaskManager.GraphQL.Tasks;

public sealed class TitleEmptyException() : Exception("The title cannot be empty.");

public sealed class UsersNotFoundException(string targetUsername) : Exception("Some users were not found");

public sealed class PermissionException() :  Exception("You are not allowed");

