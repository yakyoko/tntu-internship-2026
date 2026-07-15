using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Clients;

public class ProjectApiClient(HttpClient httpClient) : IProjectApiClient
{
    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId) =>
        await httpClient.GetFromJsonAsync<ProjectDto>($"/api/v1/projects/{projectId}");
}
