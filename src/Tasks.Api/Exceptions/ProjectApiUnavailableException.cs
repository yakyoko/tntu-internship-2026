namespace Tasks.Api.Exceptions;

public class ProjectApiUnavailableException : Exception
{
    public ProjectApiUnavailableException(Exception innerException)
        : base("Projects.Api is currently unavailable.", innerException) { }
}
