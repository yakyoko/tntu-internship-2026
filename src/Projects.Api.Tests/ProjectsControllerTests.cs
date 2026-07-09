using Microsoft.AspNetCore.Mvc;
using Moq;
using Projects.Api.Controllers;
using Projects.Api.Interfaces;
using Projects.Api.Models;

namespace Projects.Api.Tests;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectService> _mockService;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        this._mockService = new Mock<IProjectService>();
        this._controller = new ProjectsController(this._mockService.Object);
    }

    [Fact]
    public async Task GetAllProjects_ReturnsEmptyList_WhenNoProjects()
    {
        // Arrange
        this._mockService.Setup(s => s.GetAllProjectsAsync()).ReturnsAsync(new List<ProjectDto>());

        // Act
        var result = await this._controller.GetAllProjects();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetAllProjects_ReturnsProjects_WhenServiceProvidesProjects()
    {
        // Arrange
        var projects = new[]
        {
            new ProjectDto { Id = Guid.NewGuid(), Name = "A" },
            new ProjectDto { Id = Guid.NewGuid(), Name = "B" },
        };
        this._mockService.Setup(s => s.GetAllProjectsAsync()).ReturnsAsync(projects);

        // Act
        var result = await this._controller.GetAllProjects();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(ok.Value);
        Assert.Equal(2, list.Count());
    }

    [Fact]
    public async Task CreateProject_ReturnsCreated_WithLocationAndBody()
    {
        // Arrange
        var createDto = new CreateProjectDto { Name = "New", Description = "d" };
        var created = new ProjectDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            IsArchived = false,
        };

        this._mockService.Setup(s =>
                s.CreateProjectAsync(
                    It.Is<CreateProjectDto>(d =>
                        d.Name == createDto.Name && d.Description == createDto.Description
                    )
                )
            )
            .ReturnsAsync(created);

        // Act
        var result = await this._controller.CreateProject(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProjectsController.GetProjectById), createdResult.ActionName);
        Assert.Equal(created.Id, ((ProjectDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task GetProjectById_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var id = Guid.NewGuid();
        this._mockService.Setup(s => s.GetProjectByIdAsync(id)).ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await this._controller.GetProjectById(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProjectById_ReturnsOk_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var project = new ProjectDto
        {
            Id = id,
            Name = "X",
            CreatedAt = DateTime.UtcNow,
        };
        this._mockService.Setup(s => s.GetProjectByIdAsync(id)).ReturnsAsync(project);

        // Act
        var result = await this._controller.GetProjectById(id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ProjectDto>(ok.Value!);
        Assert.Equal(project.Id, body.Id);
    }
}
