using AutoMapper;
using Tasks.Api.Domain;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Services;

public class TaskService(
    ITaskRepository repository,
    IProjectApiClient apiClient,
    IMapper mapper,
    ILogger<TaskService> logger
) : ITaskService
{
    public async Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto createTaskDto)
    {
        var project = await apiClient.GetProjectByIdAsync(projectId);
        if (project is null)
        {
            logger.LogWarning("Task creation failed, project {ProjectId} not found", projectId);
            throw new ProjectNotFoundException(projectId);
        }

        if (project.IsArchived)
        {
            logger.LogWarning("Task creation rejected, project {ProjectId} is archived", projectId);
            throw new ProjectArchivedException(projectId);
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Status = TaskItemStatus.ToDo,
            Assignee = createTaskDto.Assignee,
            DueDate = createTaskDto.DueDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        await repository.CreateTaskAsync(task);
        logger.LogInformation(
            "Task {TaskId} created in project {ProjectId} with title {Title}",
            task.Id,
            projectId,
            task.Title
        );

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> GetTaskByIdAsync(Guid projectId, Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            logger.LogWarning("Task {TaskId} not found in project {ProjectId}", taskId, projectId);
            throw new TaskNotFoundException(taskId);
        }

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<IEnumerable<TaskItemDto>> GetAllTasksByProjectIdAsync(
        Guid projectId,
        TaskItemStatus? taskFilterStatus
    )
    {
        var project = await apiClient.GetProjectByIdAsync(projectId);
        if (project is null)
        {
            logger.LogWarning("Task list failed, project {ProjectId} not found", projectId);
            throw new ProjectNotFoundException(projectId);
        }

        var tasks = await repository.GetAllTasksByProjectIdAsync(projectId, taskFilterStatus);

        return mapper.Map<IEnumerable<TaskItemDto>>(tasks);
    }

    public async Task<TaskItemDto> UpdateTaskAsync(
        Guid projectId,
        Guid taskId,
        UpdateTaskDto updateTaskDto
    )
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            logger.LogWarning("Update failed, task {TaskId} not found", taskId);
            throw new TaskNotFoundException(taskId);
        }

        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.Assignee = updateTaskDto.Assignee;
        task.DueDate = updateTaskDto.DueDate;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await repository.SaveChangesAsync();
        logger.LogInformation("Task {TaskId} updated", taskId);

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> ChangeTaskStatusAsync(
        Guid projectId,
        Guid taskId,
        ChangeTaskStatusDto changeTaskStatusDto
    )
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            logger.LogWarning("Status change failed, task {TaskId} not found", taskId);
            throw new TaskNotFoundException(taskId);
        }

        var currentStatus = task.Status;
        var newStatus = changeTaskStatusDto.Status;

        if (!TaskStatusTransition.IsAllowed(currentStatus, newStatus))
        {
            logger.LogWarning(
                "Rejected invalid transition for task {TaskId}: {From} -> {To}",
                taskId,
                currentStatus,
                newStatus
            );
            throw new InvalidTaskStatusTransitionException(currentStatus, newStatus);
        }

        task.Status = newStatus;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await repository.SaveChangesAsync();

        logger.LogInformation(
            "Task {TaskId} transitioned {From} -> {To}",
            taskId,
            currentStatus,
            newStatus
        );

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task DeleteTaskAsync(Guid projectId, Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            logger.LogWarning("Delete failed, task {TaskId} not found", taskId);
            throw new TaskNotFoundException(taskId);
        }

        await repository.RemoveTaskAsync(task);
        logger.LogInformation("Task {TaskId} deleted", taskId);
    }
}
