using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tasks.Api.Infrastructure;

public class ProjectsApiHealthCheck(
    IHttpClientFactory httpClientFactory,
    ILogger<ProjectsApiHealthCheck> logger
) : IHealthCheck
{
    public const string ClientName = "ProjectsApiHealthCheck";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var client = httpClientFactory.CreateClient(ClientName);
            var response = await client.GetAsync("/health", cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Projects.Api is reachable.")
                : new HealthCheckResult(
                    context.Registration.FailureStatus,
                    "Projects.Api returned a non-success status."
                );
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Projects.Api health check failed");
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "Projects.Api is unreachable.",
                ex
            );
        }
    }
}
