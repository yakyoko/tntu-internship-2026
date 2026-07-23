using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Projects.Api.Interfaces;
using Projects.Api.Repositories;
using Projects.Api.Services;

namespace Projects.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectsApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectService, ProjectService>();
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

        services.AddDbContext<ProjectsDbContext>(dbContextOptions =>
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
