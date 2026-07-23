using Tasks.Api.Clients;
using Tasks.Api.Interfaces;

namespace Tasks.Api.Infrastructure.Extensions;

public static class ProjectApiClientExtensions
{
    public static IServiceCollection AddProjectApiClient(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpClient<IProjectApiClient, ProjectApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ProjectsApi:BaseUrl"]!);
        });

        return services;
    }
}
