namespace Projects.Api.Infrastructure.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeProjectsDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
