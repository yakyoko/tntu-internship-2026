using Microsoft.EntityFrameworkCore;
using Projects.Api.Models;

namespace Projects.Api.Infrastructure;

public class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().ToContainer("projects").HasPartitionKey(p => p.Id);
        base.OnModelCreating(modelBuilder);
    }
}
