namespace RealTaskManager.GraphQL.Tasks;

public sealed class TitleEmptyException() : Exception("The title cannot be empty.");

public sealed class TaskNotFoundException() : Exception("The task cannot be found.");