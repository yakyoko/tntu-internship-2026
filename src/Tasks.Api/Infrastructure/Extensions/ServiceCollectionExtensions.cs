using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Tasks.Api.Interfaces;
using Tasks.Api.Repositories;
using Tasks.Api.Services;

namespace Tasks.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTasksApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }

    public static IServiceCollection AddCosmosInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string accountEndpoint =
            configuration["CosmosDb:Endpoint"]
            ?? throw new InvalidOperationException("CosmosDb:Endpoint is missing.");

        string accountKey =
            configuration["CosmosDb:Key"]
            ?? throw new InvalidOperationException("CosmosDb:Key is missing.");

        string databaseName =
            configuration["CosmosDb:DatabaseName"]
            ?? throw new InvalidOperationException("CosmosDb:DatabaseName is missing.");

        services.AddDbContext<TasksDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseCosmos(
                accountEndpoint,
                accountKey,
                databaseName,
                cosmosOptions =>
                {
                    cosmosOptions.HttpClientFactory(() =>
                        new HttpClient(
                            new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback =
                                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                            }
                        )
                    );

                    cosmosOptions.ConnectionMode(ConnectionMode.Gateway);
                    cosmosOptions.LimitToEndpoint(true);
                }
            );
        });

        return services;
    }
}
