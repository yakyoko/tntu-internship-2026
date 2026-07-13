namespace Projects.Api.Exceptions;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(string message)
        : base(message) { }

    public ProjectNotFoundException(int projectId)
        : base($"Project with ID {projectId} was not found.") { }
}
