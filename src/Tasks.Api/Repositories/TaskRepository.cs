using Microsoft.EntityFrameworkCore;
using Tasks.Api.Infrastructure;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TasksDbContext _context;
    private readonly DbSet<TaskItem> _tasksDbSet;

    public TaskRepository(TasksDbContext context)
    {
        this._context = context;
        this._tasksDbSet = context.Set<TaskItem>();
    }

    public async Task CreateTaskAsync(TaskItem task)
    {
        await this._tasksDbSet.AddAsync(task);
        await this._context.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid projectId, Guid taskId) =>
        await this
            ._tasksDbSet.WithPartitionKey(projectId.ToString())
            .FirstOrDefaultAsync(t => t.Id == taskId);

    public async Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(Guid projectId) =>
        await this
            ._tasksDbSet.WithPartitionKey(projectId.ToString())
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task SaveChangesAsync()
    {
        await this._context.SaveChangesAsync();
    }

    public async Task RemoveTaskAsync(TaskItem task)
    {
        this._tasksDbSet.Remove(task);
        await this._context.SaveChangesAsync();
    }
}
