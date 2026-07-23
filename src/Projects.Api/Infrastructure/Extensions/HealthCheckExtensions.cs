using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Projects.Api.Infrastructure.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddProjectsHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<CosmosDbHealthCheck>(name: "cosmosdb", failureStatus: HealthStatus.Unhealthy);

        return services;
    }

    public static IEndpointRouteBuilder MapProjectsHealthChecks(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                        }),
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                },
            }
        );

        return endpoints;
    }
}
