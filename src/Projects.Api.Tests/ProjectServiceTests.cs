using AutoMapper;
using Moq;
using Projects.Api.Interfaces;
using Projects.Api.Models;
using Projects.Api.Services;

namespace Projects.Api.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        this._repoMock = new Mock<IProjectRepository>();
        this._mapperMock = new Mock<IMapper>();
        this._service = new ProjectService(this._repoMock.Object, this._mapperMock.Object);
    }

    [Fact]
    public async Task CreateProjectAsync_CreatesProjectAndReturnsDto()
    {
        // Arrange
        var create = new CreateProjectDto { Name = "N", Description = "D" };
        this._repoMock.Setup(r => r.CreateProjectAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);
        this._mapperMock.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns<Project>(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                IsArchived = p.IsArchived,
            });

        // Act
        var result = await this._service.CreateProjectAsync(create);

        // Assert
        this._repoMock.Verify(r => r.CreateProjectAsync(It.IsAny<Project>()), Times.Once);
        Assert.Equal(create.Name, result.Name);
        Assert.Equal(create.Description, result.Description);
        Assert.False(result.IsArchived);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        this._repoMock.Setup(r => r.GetProjectByIdAsync(id)).ReturnsAsync((Project?)null);

        // Act
        var result = await this._service.GetProjectByIdAsync(id);

        // Assert
        Assert.Null(result);
        this._repoMock.Verify(r => r.GetProjectByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsDto_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var project = new Project
        {
            Id = id,
            Name = "X",
            CreatedAt = DateTime.UtcNow,
        };
        this._repoMock.Setup(r => r.GetProjectByIdAsync(id)).ReturnsAsync(project);
        this._mapperMock.Setup(m => m.Map<ProjectDto?>(project))
            .Returns(
                new ProjectDto
                {
                    Id = id,
                    Name = project.Name,
                    CreatedAt = project.CreatedAt,
                }
            );

        // Act
        var result = await this._service.GetProjectByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        this._repoMock.Verify(r => r.GetProjectByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllProjectsAsync_MapsAndReturnsAll()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var projects = new[]
        {
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "A",
                CreatedAt = now,
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "B",
                CreatedAt = now.AddMinutes(-1),
            },
        };
        this._repoMock.Setup(r => r.GetAllProjectsAsync()).ReturnsAsync(projects);
        this._mapperMock.Setup(m =>
                m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>())
            )
            .Returns<IEnumerable<Project>>(ps =>
                ps.Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        CreatedAt = p.CreatedAt,
                    })
                    .ToList()
            );

        // Act
        var result = await this._service.GetAllProjectsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        this._repoMock.Verify(r => r.GetAllProjectsAsync(), Times.Once);
    }
}
