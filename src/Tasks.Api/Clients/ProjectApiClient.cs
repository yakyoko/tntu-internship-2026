using System.Net;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Clients;

public class ProjectApiClient(HttpClient httpClient, ILogger<ProjectApiClient> logger)
    : IProjectApiClient
{
    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId)
    {
        try
        {
            var project = await httpClient.GetFromJsonAsync<ProjectDto>(
                $"/api/v1/projects/{projectId}"
            );
            return project;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogInformation(ex, "Project {ProjectId} not found in Projects.Api", projectId);
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "Projects.Api call failed for project {ProjectId} with status {StatusCode}",
                projectId,
                ex.StatusCode
            );
            throw new ProjectApiUnavailableException(ex);
        }
        catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Projects.Api call timed out for project {ProjectId}", projectId);
            throw new ProjectApiUnavailableException(ex);
        }
    }
}
