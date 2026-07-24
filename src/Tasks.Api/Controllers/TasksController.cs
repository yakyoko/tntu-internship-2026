using Microsoft.AspNetCore.Mvc;
using Tasks.Api.Exceptions;
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
        try
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
        catch (ProjectNotFoundException ex)
        {
            logger.LogWarning(ex, "Task creation failed, project {ProjectId} not found", projectId);
            return this.NotFound(ex.Message);
        }
        catch (ProjectArchivedException ex)
        {
            logger.LogWarning(
                ex,
                "Task creation rejected, project {ProjectId} is archived",
                projectId
            );
            return this.Conflict(ex.Message);
        }
        catch (ProjectApiUnavailableException ex)
        {
            logger.LogError(
                ex,
                "Task creation failed, Projects.Api unavailable for project {ProjectId}",
                projectId
            );
            return this.StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
    }

    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid projectId, Guid taskId)
    {
        try
        {
            var task = await service.GetTaskByIdAsync(projectId, taskId);
            return this.Ok(task);
        }
        catch (TaskNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Task {TaskId} not found in project {ProjectId}",
                taskId,
                projectId
            );
            return this.NotFound(ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetAllTasksByProjectId(Guid projectId)
    {
        try
        {
            var tasks = await service.GetAllTasksByProjectIdAsync(projectId);
            return this.Ok(tasks);
        }
        catch (ProjectNotFoundException ex)
        {
            logger.LogWarning(ex, "Task list failed, project {ProjectId} not found", projectId);
            return this.NotFound(ex.Message);
        }
        catch (ProjectApiUnavailableException ex)
        {
            logger.LogError(
                ex,
                "Task list failed, Projects.Api unavailable for project {ProjectId}",
                projectId
            );
            return this.StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
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
        try
        {
            var task = await service.UpdateTaskAsync(projectId, taskId, updateTaskDto);
            logger.LogInformation("Task {TaskId} updated", taskId);
            return this.Ok(task);
        }
        catch (TaskNotFoundException ex)
        {
            logger.LogWarning(ex, "Update failed, task {TaskId} not found", taskId);
            return this.NotFound(ex.Message);
        }
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
        try
        {
            var task = await service.ChangeTaskStatusAsync(projectId, taskId, status);
            logger.LogInformation(
                "Task {TaskId} status changed to {Status}",
                taskId,
                status.Status
            );
            return this.Ok(task);
        }
        catch (TaskNotFoundException ex)
        {
            logger.LogWarning(ex, "Status change failed, task {TaskId} not found", taskId);
            return this.NotFound(ex.Message);
        }
        catch (InvalidTaskStatusTransitionException ex)
        {
            logger.LogWarning(
                ex,
                "Status change rejected for task {TaskId}: attempted {AttemptedStatus}",
                taskId,
                status.Status
            );
            return this.Conflict(ex.Message);
        }
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
    {
        try
        {
            await service.DeleteTaskAsync(projectId, taskId);
            logger.LogInformation("Task {TaskId} deleted", taskId);
            return this.NoContent();
        }
        catch (TaskNotFoundException ex)
        {
            logger.LogWarning(ex, "Delete failed, task {TaskId} not found", taskId);
            return this.NotFound(ex.Message);
        }
    }
}
