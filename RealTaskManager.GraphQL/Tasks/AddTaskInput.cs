using RealTaskManager.Core.Entities;

namespace RealTaskManager.GraphQL.Tasks;

public record AddTaskInput(
    string Title,
    string? Description,
    string? Status,
    Guid? AssignedToUserId,
    Guid? CreatedByUserId
);