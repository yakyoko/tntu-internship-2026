using AutoMapper;
using Tasks.Api.Domain;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Services;

public class TaskService(ITaskRepository repository, IProjectApiClient apiClient, IMapper mapper)
    : ITaskService
{
    public async Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto createTaskDto)
    {
        var project = await apiClient.GetProjectByIdAsync(projectId);
        if (project is null)
        {
            throw new ProjectNotFoundException(projectId);
        }

        if (project.IsArchived)
        {
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

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> GetTaskByIdAsync(Guid projectId, Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            throw new TaskNotFoundException(taskId);
        }
        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<IEnumerable<TaskItemDto>> GetAllTasksByProjectIdAsync(Guid projectId)
    {
        var project = await apiClient.GetProjectByIdAsync(projectId);
        if (project is null)
        {
            throw new ProjectNotFoundException(projectId);
        }

        var tasks = await repository.GetAllTasksByProjectIdAsync(projectId);
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
            throw new TaskNotFoundException(taskId);
        }

        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.Assignee = updateTaskDto.Assignee;
        task.DueDate = updateTaskDto.DueDate;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await repository.SaveChangesAsync();

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
            throw new TaskNotFoundException(taskId);
        }

        var currentStatus = task.Status;
        var newStatus = changeTaskStatusDto.Status;

        if (!TaskStatusTransition.IsAllowed(currentStatus, newStatus))
        {
            throw new InvalidTaskStatusTransitionException(currentStatus, newStatus);
        }

        task.Status = newStatus;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await repository.SaveChangesAsync();
        return mapper.Map<TaskItemDto>(task);
    }

    public async Task DeleteTaskAsync(Guid projectId, Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);
        if (task is null)
        {
            throw new TaskNotFoundException(taskId);
        }
        await repository.RemoveTaskAsync(task);
    }
}
