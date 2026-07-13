using AutoMapper;
using Projects.Api.Exceptions;
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

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await repository.GetAllProjectsAsync();
        return mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectDto projectDto)
    {
        var project = await repository.GetProjectByIdAsync(id);

        if (project is null)
        {
            throw new ProjectNotFoundException(id);
        }

        if (project.IsArchived)
        {
            throw new ProjectArchivedException(id);
        }

        project.Name = projectDto.Name;
        project.Description = projectDto.Description;

        await repository.SaveChangesAsync();
        return mapper.Map<ProjectDto>(project);
    }
}
