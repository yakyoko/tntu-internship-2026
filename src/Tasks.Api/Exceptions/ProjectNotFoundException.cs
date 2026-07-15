namespace Tasks.Api.Exceptions;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(string message)
        : base(message) { }

    public ProjectNotFoundException(Guid projectId)
        : base($"Project with ID {projectId} was not found.") { }
}
