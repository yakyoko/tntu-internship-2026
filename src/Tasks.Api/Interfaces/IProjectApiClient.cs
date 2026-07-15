using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface IProjectApiClient
{
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId);
}
