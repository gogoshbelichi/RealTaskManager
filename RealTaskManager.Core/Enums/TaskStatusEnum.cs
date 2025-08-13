namespace RealTaskManager.Core.Entities;

public enum TaskStatusEnum
{
    Backlog, // 1 because it's stored in db as string values the order is like this
    Todo, // 6
    InProgress, // 4
    Done, // 3
    Suspended, // 5
    Cancelled, // 2
}