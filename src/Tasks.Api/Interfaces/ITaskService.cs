using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto createTaskDto);
    Task<TaskItemDto> GetTaskByIdAsync(Guid projectId, Guid taskId);
    Task<IEnumerable<TaskItemDto>> GetAllTasksByProjectIdAsync(Guid projectId);
    Task<TaskItemDto> UpdateTaskAsync(Guid projectId, Guid taskId, UpdateTaskDto updateTaskDto);
    Task<TaskItemDto> ChangeTaskStatusAsync(
        Guid projectId,
        Guid taskId,
        ChangeTaskStatusDto changeTaskStatusDto
    );

    Task DeleteTaskAsync(Guid projectId, Guid taskId);
}
