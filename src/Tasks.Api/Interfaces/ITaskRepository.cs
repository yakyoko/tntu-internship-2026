using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskRepository
{
    Task CreateTaskAsync(TaskItem task);
    Task<TaskItem?> GetTaskByIdAsync(Guid projectId, Guid taskId);
    Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(Guid projectId);
    Task SaveChangesAsync();
}
