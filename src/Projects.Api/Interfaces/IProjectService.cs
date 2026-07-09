using Projects.Api.Models;

namespace Projects.Api.Interfaces;

public interface IProjectService
{
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto);
    Task<ProjectDto?> GetProjectByIdAsync(Guid id);
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
}
