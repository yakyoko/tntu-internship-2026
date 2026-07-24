using Microsoft.AspNetCore.Mvc;
using Projects.Api.Exceptions;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController(IProjectService service, ILogger<ProjectsController> logger)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(CreateProjectDto projectDto)
    {
        var project = await service.CreateProjectAsync(projectDto);
        logger.LogInformation(
            "Project {ProjectId} created with name {ProjectName}",
            project.Id,
            project.Name
        );
        return this.CreatedAtAction(nameof(this.GetProjectById), new { id = project.Id }, project);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await service.GetProjectByIdAsync(id);
        return project is null ? this.NotFound() : this.Ok(project);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await service.GetAllProjectsAsync();
        return this.Ok(projects);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto projectDto)
    {
        try
        {
            var project = await service.UpdateProjectAsync(id, projectDto);
            logger.LogInformation("Project {ProjectId} updated", id);
            return this.Ok(project);
        }
        catch (ProjectNotFoundException ex)
        {
            logger.LogWarning(ex, "Update failed, project {ProjectId} not found", id);
            return this.NotFound(ex.Message);
        }
        catch (ProjectArchivedException ex)
        {
            logger.LogWarning(ex, "Update rejected, project {ProjectId} is archived", id);
            return this.Conflict(ex.Message);
        }
    }

    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ArchiveProject(Guid id)
    {
        try
        {
            var project = await service.ArchiveProjectAsync(id);
            logger.LogInformation("Project {ProjectId} archived", id);
            return this.Ok(project);
        }
        catch (ProjectNotFoundException ex)
        {
            logger.LogWarning(ex, "Archive failed, project {ProjectId} not found", id);
            return this.NotFound(ex.Message);
        }
        catch (ProjectArchivedException ex)
        {
            logger.LogWarning(ex, "Archive rejected, project {ProjectId} already archived", id);
            return this.Conflict(ex.Message);
        }
    }
}
