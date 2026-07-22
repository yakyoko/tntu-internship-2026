using Tasks.Api.Models;

namespace Tasks.Api.Domain;

public static class TaskStatusTransition
{
    private static readonly Dictionary<TaskItemStatus, TaskItemStatus[]> AllowedTransitions = new()
    {
        [TaskItemStatus.ToDo] = [TaskItemStatus.InProgress],
        [TaskItemStatus.InProgress] = [TaskItemStatus.Done],
        [TaskItemStatus.Done] = [],
    };

    public static bool IsAllowed(TaskItemStatus from, TaskItemStatus to) =>
        AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
}
