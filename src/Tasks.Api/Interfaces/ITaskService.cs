using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto createTaskDto);
    Task<TaskItemDto?> GetTaskByIdAsync(Guid projectId, Guid taskId);
    Task<IEnumerable<TaskItemDto>> GetAllTasksByProjectIdAsync(Guid projectId);
}
