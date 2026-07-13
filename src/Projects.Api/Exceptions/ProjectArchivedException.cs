namespace Projects.Api.Exceptions;

public class ProjectArchivedException : Exception
{
    public ProjectArchivedException(string message)
        : base(message) { }

    public ProjectArchivedException(Guid projectId)
        : base($"Project with ID {projectId} is archived and cannot be modified.") { }
}
