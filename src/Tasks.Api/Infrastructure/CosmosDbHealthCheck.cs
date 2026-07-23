using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tasks.Api.Infrastructure;

public class CosmosDbHealthCheck(
    IServiceScopeFactory scopeFactory,
    ILogger<CosmosDbHealthCheck> logger
) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();

            await dbContext.TaskItems.Take(1).ToListAsync(cancellationToken);

            return HealthCheckResult.Healthy("Cosmos DB is reachable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cosmos DB health check failed");
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "Cosmos DB is unreachable.",
                ex
            );
        }
    }
}
