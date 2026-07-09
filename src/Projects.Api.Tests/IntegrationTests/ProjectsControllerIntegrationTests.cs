using System.Net;
using System.Net.Http.Json;
using Projects.Api.Models;

namespace Projects.Api.Tests.IntegrationTests;

public class ProjectsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndOnlyNonArchivedProjects()
    {
        // Act
        var resp = await _client.GetAsync("/api/v1/projects");

        // Assert
        resp.EnsureSuccessStatusCode();
        var items = await resp.Content.ReadFromJsonAsync<ProjectDto[]>();
        Assert.NotNull(items);
        Assert.All(items!, p => Assert.False(p.IsArchived));
        Assert.True(items!.Length >= 1);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProjectExists()
    {
        // Arrange
        var seededId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var resp = await _client.GetAsync($"/api/v1/projects/{seededId}");

        // Assert
        resp.EnsureSuccessStatusCode();
        var project = await resp.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);
        Assert.Equal(seededId, project!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        // Act
        var resp = await _client.GetAsync($"/api/v1/projects/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated_And_GetByIdReturnsCreated()
    {
        // Arrange
        var create = new CreateProjectDto { Name = "IT Integration", Description = "int test" };

        // Act - create
        var post = await _client.PostAsJsonAsync("/api/v1/projects", create);

        // Assert create
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var created = await post.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(created);
        Assert.Equal(create.Name, created!.Name);

        // Act - get by id
        var get = await _client.GetAsync($"/api/v1/projects/{created.Id}");

        // Assert get
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenInvalid()
    {
        // Arrange - missing required Name
        var invalid = new CreateProjectDto { Name = "", Description = "bad" };

        // Act
        var resp = await _client.PostAsJsonAsync("/api/v1/projects", invalid);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}
