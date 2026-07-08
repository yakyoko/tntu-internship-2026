using AutoMapper;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Services;

public class ProjectService(IProjectRepository repository, IMapper mapper) : IProjectService
{
    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto)
    {
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = projectDto.Name,
            Description = projectDto.Description,
            CreatedAt = DateTime.UtcNow,
            IsArchived = false,
        };

        await repository.CreateProjectAsync(project);
        return mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id)
    {
        var project = await repository.GetProjectByIdAsync(id);
        return mapper.Map<ProjectDto?>(project);
    }
}
