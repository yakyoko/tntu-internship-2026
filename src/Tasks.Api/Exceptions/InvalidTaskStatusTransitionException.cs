using Tasks.Api.Models;

namespace Tasks.Api.Exceptions;

public class InvalidTaskStatusTransitionException : Exception
{
    public InvalidTaskStatusTransitionException(string message)
        : base(message) { }

    public InvalidTaskStatusTransitionException(TaskItemStatus from, TaskItemStatus to)
        : base($"Cannot transition task from '{from}' to '{to}'.") { }
}
