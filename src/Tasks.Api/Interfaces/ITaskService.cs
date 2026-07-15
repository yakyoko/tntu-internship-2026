using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto createTaskDto);
}
