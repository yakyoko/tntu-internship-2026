using AutoMapper;
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
            Status = "ToDo",
            Assignee = createTaskDto.Assignee,
            DueDate = createTaskDto.DueDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        await repository.CreateTaskAsync(task);

        return mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto?> GetTaskByIdAsync(Guid projectId, Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(projectId, taskId);

        if (task is null || task.ProjectId != projectId)
        {
            return null;
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
}
