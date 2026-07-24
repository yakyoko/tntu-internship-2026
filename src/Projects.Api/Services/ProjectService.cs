using AutoMapper;
using Projects.Api.Exceptions;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Services;

public class ProjectService(
    IProjectRepository repository,
    IMapper mapper,
    ILogger<ProjectService> logger
) : IProjectService
{
    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto)
    {
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = projectDto.Name,
            Description = projectDto.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            IsArchived = false,
        };

        await repository.CreateProjectAsync(project);
        logger.LogInformation(
            "Project {ProjectId} created with name {ProjectName}",
            project.Id,
            project.Name
        );

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
            logger.LogWarning("Update failed, project {ProjectId} not found", id);
            throw new ProjectNotFoundException(id);
        }

        if (project.IsArchived)
        {
            logger.LogWarning("Update rejected, project {ProjectId} is archived", id);
            throw new ProjectArchivedException(id);
        }

        project.Name = projectDto.Name;
        project.Description = projectDto.Description;

        await repository.SaveChangesAsync();

        logger.LogInformation(
            "Project {ProjectId} updated with name {ProjectName}",
            id,
            project.Name
        );

        return mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> ArchiveProjectAsync(Guid id)
    {
        var project = await repository.GetProjectByIdAsync(id);
        if (project is null)
        {
            logger.LogWarning("Archive failed, project {ProjectId} not found", id);
            throw new ProjectNotFoundException(id);
        }

        if (project.IsArchived)
        {
            logger.LogWarning("Archive rejected, project {ProjectId} already archived", id);
            throw new ProjectArchivedException(id);
        }

        project.IsArchived = true;
        await repository.SaveChangesAsync();

        logger.LogInformation("Project {ProjectId} archived", id);

        return mapper.Map<ProjectDto>(project);
    }
}
