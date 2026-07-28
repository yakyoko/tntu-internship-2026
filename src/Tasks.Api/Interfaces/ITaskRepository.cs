using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskRepository
{
    Task CreateTaskAsync(TaskItem task);
    Task<TaskItem?> GetTaskByIdAsync(Guid projectId, Guid taskId);
    Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(
        Guid projectId,
        TaskItemStatus? status = null
    );
    Task SaveChangesAsync();
    Task RemoveTaskAsync(TaskItem task);
}
