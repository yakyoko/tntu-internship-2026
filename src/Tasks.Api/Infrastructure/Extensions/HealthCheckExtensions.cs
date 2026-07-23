using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tasks.Api.Infrastructure.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddTasksHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpClient(
            ProjectsApiHealthCheck.ClientName,
            client =>
            {
                client.BaseAddress = new Uri(configuration["ProjectsApi:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(5);
            }
        );

        services
            .AddHealthChecks()
            .AddCheck<CosmosDbHealthCheck>(name: "cosmosdb", failureStatus: HealthStatus.Unhealthy)
            .AddCheck<ProjectsApiHealthCheck>(
                name: "projectsapi",
                failureStatus: HealthStatus.Degraded
            );

        return services;
    }

    public static IEndpointRouteBuilder MapTasksHealthChecks(this IEndpointRouteBuilder endpoints)
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
