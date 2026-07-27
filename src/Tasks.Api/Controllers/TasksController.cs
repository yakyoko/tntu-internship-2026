using Microsoft.AspNetCore.Mvc;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Controllers;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/tasks")]
public class TasksController(ITaskService service, ILogger<TasksController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CreateTask(Guid projectId, CreateTaskDto createTaskDto)
    {
        var task = await service.CreateTaskAsync(projectId, createTaskDto);
        logger.LogInformation(
            "Task {TaskId} created in project {ProjectId} with title {Title}",
            task.Id,
            projectId,
            task.Title
        );

        return this.CreatedAtAction(
            nameof(this.GetTaskById),
            new { projectId, taskId = task.Id },
            task
        );
    }

    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid projectId, Guid taskId)
    {
        var task = await service.GetTaskByIdAsync(projectId, taskId);
        return this.Ok(task);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetAllTasksByProjectId(Guid projectId)
    {
        var tasks = await service.GetAllTasksByProjectIdAsync(projectId);
        return this.Ok(tasks);
    }

    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(
        Guid projectId,
        Guid taskId,
        UpdateTaskDto updateTaskDto
    )
    {
        var task = await service.UpdateTaskAsync(projectId, taskId, updateTaskDto);
        logger.LogInformation("Task {TaskId} updated", taskId);
        return this.Ok(task);
    }

    [HttpPatch("{taskId:guid}/status")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeTaskStatus(
        Guid projectId,
        Guid taskId,
        ChangeTaskStatusDto status
    )
    {
        var task = await service.ChangeTaskStatusAsync(projectId, taskId, status);
        logger.LogInformation("Task {TaskId} status changed to {Status}", taskId, status.Status);
        return this.Ok(task);
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
    {
        await service.DeleteTaskAsync(projectId, taskId);
        logger.LogInformation("Task {TaskId} deleted", taskId);
        return this.NoContent();
    }
}
