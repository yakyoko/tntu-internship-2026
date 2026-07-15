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
}
