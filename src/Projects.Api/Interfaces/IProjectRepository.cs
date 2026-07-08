using Projects.Api.Models;

namespace Projects.Api.Interfaces;

public interface IProjectRepository
{
    Task CreateProjectAsync(Project project);
    Task<Project?> GetProjectByIdAsync(Guid id);
}
