namespace RealTaskManager.GraphQL.Tasks;

public sealed record UsersNotAssignedError(
    string Message = "Cannot not assign to the task");
    
public sealed record TaskNotCreatedError(
    string Message = "Unable to create task right now");
    
public sealed record TaskNotFoundError(
    string Message = "The task cannot be found");
    
public sealed record UserNotFoundError(
    string Message = "The user cannot be found");
    
public sealed record TaskAlreadyAssignedError(
    string Message = "The task is already assigned to you");  