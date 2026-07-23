namespace Tasks.Api.Infrastructure.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeTasksDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
