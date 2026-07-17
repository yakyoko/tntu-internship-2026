using Microsoft.AspNetCore.Mvc;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Controllers;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/tasks")]
public class TasksController(ITaskService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTask(Guid projectId, CreateTaskDto createTaskDto)
    {
        try
        {
            var task = await service.CreateTaskAsync(projectId, createTaskDto);
            return this.CreatedAtAction(
                nameof(this.GetTaskById),
                new { projectId, taskId = task.Id },
                task
            );
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

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetTaskById(Guid projectId, Guid taskId)
    {
        var task = await service.GetTaskByIdAsync(projectId, taskId);
        return task is null ? this.NotFound() : this.Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasksByProjectId(Guid projectId)
    {
        try
        {
            var tasks = await service.GetAllTasksByProjectIdAsync(projectId);
            return this.Ok(tasks);
        }
        catch (ProjectNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
    }
}
