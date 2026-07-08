using Microsoft.EntityFrameworkCore;
using Projects.Api.Infrastructure;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectsDbContext _context;
    private readonly DbSet<Project> _projectsDbSet;

    public ProjectRepository(ProjectsDbContext context)
    {
        this._context = context;
        this._projectsDbSet = context.Set<Project>();
    }

    public async Task CreateProjectAsync(Project project)
    {
        await this._projectsDbSet.AddAsync(project);
        await this._context.SaveChangesAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        return await this._projectsDbSet.FindAsync(id);
    }
}
