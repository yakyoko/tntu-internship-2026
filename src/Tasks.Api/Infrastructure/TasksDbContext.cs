using Microsoft.EntityFrameworkCore;
using Tasks.Api.Models;

namespace Tasks.Api.Infrastructure;

public class TasksDbContext(DbContextOptions<TasksDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().ToContainer("tasks").HasPartitionKey(t => t.ProjectId);
        modelBuilder.Entity<TaskItem>().Property(t => t.Status).HasConversion<string>();
        base.OnModelCreating(modelBuilder);
    }
}
