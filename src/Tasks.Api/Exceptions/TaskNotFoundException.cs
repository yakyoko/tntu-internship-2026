namespace Tasks.Api.Exceptions;

public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(string message)
        : base(message) { }

    public TaskNotFoundException(Guid taskId)
        : base($"Task with id '{taskId}' was not found.") { }
}
