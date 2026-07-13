using Microsoft.AspNetCore.Mvc;
using Projects.Api.Exceptions;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(CreateProjectDto projectDto)
    {
        var project = await service.CreateProjectAsync(projectDto);
        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await service.GetProjectByIdAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await service.GetAllProjectsAsync();
        return Ok(projects);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto projectDto)
    {
        try
        {
            var project = await service.UpdateProjectAsync(id, projectDto);
            return this.Ok(project);
        }
        catch (ProjectNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (ProjectArchivedException ex)
        {
            return this.Conflict(ex.Message);
        }
    }
}
