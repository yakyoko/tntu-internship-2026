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
        Assert.Equal(2, items!.Length);
        Assert.Contains(items!, p => p.Id == Guid.Parse("11111111-1111-1111-1111-111111111111"));
        Assert.Contains(items!, p => p.Id == Guid.Parse("33333333-3333-3333-3333-333333333333"));
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

    [Fact]
    public async Task Archive_ReturnsOk_And_ProjectNoLongerInGetAll_ButGetByIdShowsArchived()
    {
        // Arrange
        var create = await _client.PostAsJsonAsync(
            "/api/v1/projects",
            new CreateProjectDto { Name = "To Archive" }
        );
        var created = await create.Content.ReadFromJsonAsync<ProjectDto>();
        var id = created!.Id;

        // Act - archive
        var patch = new HttpRequestMessage(
            new HttpMethod("PATCH"),
            $"/api/v1/projects/{id}/archive"
        );
        var patchResp = await _client.SendAsync(patch);

        // Assert archive success
        patchResp.EnsureSuccessStatusCode();
        var archived = await patchResp.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(archived);
        Assert.True(archived!.IsArchived);
        Assert.Equal(id, archived.Id);

        // Act - get by id
        var getResp = await _client.GetAsync($"/api/v1/projects/{id}");
        getResp.EnsureSuccessStatusCode();
        var got = await getResp.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(got);
        Assert.True(got!.IsArchived);
    }

    [Fact]
    public async Task Archive_ReturnsNotFound_WhenMissing()
    {
        // Act
        var patch = new HttpRequestMessage(
            new HttpMethod("PATCH"),
            $"/api/v1/projects/{Guid.NewGuid()}/archive"
        );
        var resp = await _client.SendAsync(patch);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Archive_ReturnsConflict_WhenAlreadyArchived()
    {
        // Arrange
        var id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Act
        var patch = new HttpRequestMessage(
            new HttpMethod("PATCH"),
            $"/api/v1/projects/{id}/archive"
        );
        var resp = await _client.SendAsync(patch);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk_And_ProjectIsUpdated()
    {
        // Arrange
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var update = new UpdateProjectDto { Name = "Updated Name", Description = "Updated Desc" };

        // Act
        var put = await _client.PutAsJsonAsync($"/api/v1/projects/{id}", update);

        // Assert
        put.EnsureSuccessStatusCode();
        var updated = await put.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(updated);
        Assert.Equal(update.Name, updated!.Name);
        Assert.Equal(update.Description, updated.Description);

        // Act - get by id
        var get = await _client.GetAsync($"/api/v1/projects/{id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(fetched);
        Assert.Equal(update.Name, fetched!.Name);
        Assert.Equal(update.Description, fetched.Description);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var id = Guid.NewGuid();
        var update = new UpdateProjectDto { Name = "X", Description = "Y" };

        // Act
        var put = await _client.PutAsJsonAsync($"/api/v1/projects/{id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsConflict_WhenArchived()
    {
        // Arrange
        var id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var update = new UpdateProjectDto { Name = "X", Description = "Y" };

        // Act
        var put = await _client.PutAsJsonAsync($"/api/v1/projects/{id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, put.StatusCode);
    }
}
