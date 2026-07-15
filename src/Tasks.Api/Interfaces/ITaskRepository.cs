using Tasks.Api.Models;

namespace Tasks.Api.Interfaces;

public interface ITaskRepository
{
    Task CreateTaskAsync(TaskItem task);
}
