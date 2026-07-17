using AutoMapper;
using Moq;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;
using Tasks.Api.Services;

namespace Tasks.Api.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock = new();
    private readonly Mock<IProjectApiClient> _projectApiClientMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _service = new TaskService(
            _repositoryMock.Object,
            _projectApiClientMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateTaskAsync_CreatesTaskAndReturnsDto_WhenProjectExists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto
        {
            Title = "Implement task",
            Description = "Task description",
            Assignee = "Alex",
            DueDate = DateTimeOffset.UtcNow.AddDays(2),
        };
        var project = new ProjectDto
        {
            Id = projectId,
            Name = "Project A",
            IsArchived = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        TaskItem? createdTask = null;

        _projectApiClientMock.Setup(c => c.GetProjectByIdAsync(projectId)).ReturnsAsync(project);

        _repositoryMock
            .Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(task => createdTask = task)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TaskItemDto>(It.IsAny<TaskItem>()))
            .Returns<TaskItem>(task => new TaskItemDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Assignee = task.Assignee,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
            });

        // Act
        var result = await _service.CreateTaskAsync(projectId, createDto);

        // Assert
        Assert.NotNull(createdTask);
        Assert.Equal(projectId, createdTask!.ProjectId);
        Assert.Equal(createDto.Title, createdTask.Title);
        Assert.Equal(createDto.Description, createdTask.Description);
        Assert.Equal(createDto.Assignee, createdTask.Assignee);
        Assert.Equal(createDto.DueDate, createdTask.DueDate);
        Assert.Equal("ToDo", createdTask.Status);
        Assert.NotEqual(Guid.Empty, createdTask.Id);
        Assert.NotEqual(default, createdTask.CreatedAt);
        Assert.NotEqual(default, createdTask.UpdatedAt);
        Assert.True(createdTask.UpdatedAt >= createdTask.CreatedAt);

        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(createDto.Title, result.Title);
        Assert.Equal("ToDo", result.Status);

        _projectApiClientMock.Verify(c => c.GetProjectByIdAsync(projectId), Times.Once);
        _repositoryMock.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_ThrowsProjectNotFoundException_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        _projectApiClientMock
            .Setup(c => c.GetProjectByIdAsync(projectId))
            .ReturnsAsync((ProjectDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _service.CreateTaskAsync(projectId, createDto)
        );

        _repositoryMock.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateTaskAsync_ThrowsProjectArchivedException_WhenProjectArchived()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };
        var project = new ProjectDto
        {
            Id = projectId,
            Name = "Archived",
            IsArchived = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _projectApiClientMock.Setup(c => c.GetProjectByIdAsync(projectId)).ReturnsAsync(project);

        // Act & Assert
        await Assert.ThrowsAsync<ProjectArchivedException>(() =>
            _service.CreateTaskAsync(projectId, createDto)
        );

        _repositoryMock.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsDto_WhenTaskFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Task",
            Description = "Desc",
            Status = "ToDo",
            Assignee = "Alex",
            DueDate = DateTimeOffset.UtcNow.AddDays(1),
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
        };

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(projectId, taskId)).ReturnsAsync(task);
        _mapperMock
            .Setup(m => m.Map<TaskItemDto>(task))
            .Returns(
                new TaskItemDto
                {
                    Id = task.Id,
                    ProjectId = task.ProjectId,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Assignee = task.Assignee,
                    DueDate = task.DueDate,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                }
            );

        // Act
        var result = await _service.GetTaskByIdAsync(projectId, taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result!.Id);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal("Task", result.Title);

        _repositoryMock.Verify(r => r.GetTaskByIdAsync(projectId, taskId), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetTaskByIdAsync(projectId, taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _service.GetTaskByIdAsync(projectId, taskId);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetTaskByIdAsync(projectId, taskId), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsNull_WhenTaskBelongsToAnotherProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetTaskByIdAsync(projectId, taskId))
            .ReturnsAsync(
                new TaskItem
                {
                    Id = taskId,
                    ProjectId = otherProjectId,
                    Title = "Task",
                    Status = "ToDo",
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                }
            );

        // Act
        var result = await _service.GetTaskByIdAsync(projectId, taskId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllTasksByProjectIdAsync_ReturnsPopulatedList_WhenTasksExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectDto
        {
            Id = projectId,
            Name = "Project A",
            IsArchived = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        var tasks = new[]
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Task 1",
                Status = "ToDo",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Task 2",
                Status = "InProgress",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            },
        };

        _projectApiClientMock.Setup(c => c.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
        _repositoryMock.Setup(r => r.GetAllTasksByProjectIdAsync(projectId)).ReturnsAsync(tasks);
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TaskItemDto>>(tasks))
            .Returns(
                tasks.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                })
            );

        // Act
        var result = await _service.GetAllTasksByProjectIdAsync(projectId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, dto => Assert.Equal(projectId, dto.ProjectId));
        _repositoryMock.Verify(r => r.GetAllTasksByProjectIdAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksByProjectIdAsync_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectDto
        {
            Id = projectId,
            Name = "Project A",
            IsArchived = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _projectApiClientMock.Setup(c => c.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
        _repositoryMock
            .Setup(r => r.GetAllTasksByProjectIdAsync(projectId))
            .ReturnsAsync(Enumerable.Empty<TaskItem>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TaskItemDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(Enumerable.Empty<TaskItemDto>());

        // Act
        var result = await _service.GetAllTasksByProjectIdAsync(projectId);

        // Assert
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetAllTasksByProjectIdAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksByProjectIdAsync_ThrowsProjectNotFoundException_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _projectApiClientMock
            .Setup(c => c.GetProjectByIdAsync(projectId))
            .ReturnsAsync((ProjectDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _service.GetAllTasksByProjectIdAsync(projectId)
        );

        _repositoryMock.Verify(r => r.GetAllTasksByProjectIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
