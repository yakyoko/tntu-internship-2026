using Microsoft.AspNetCore.Mvc;
using Moq;
using Tasks.Api.Controllers;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock = new();
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _controller = new TasksController(_serviceMock.Object);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAtAction_WhenTaskCreated()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "New task", Description = "Desc" };
        var created = new TaskItemDto
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = createDto.Title,
            Description = createDto.Description,
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _serviceMock.Setup(s => s.CreateTaskAsync(projectId, createDto)).ReturnsAsync(created);

        // Act
        var result = await _controller.CreateTask(projectId, createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(TasksController.GetTaskById), createdResult.ActionName);
        Assert.Equal(projectId, createdResult.RouteValues!["projectId"]);
        Assert.Equal(created.Id, createdResult.RouteValues!["taskId"]);

        var body = Assert.IsType<TaskItemDto>(createdResult.Value);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal(createDto.Title, body.Title);
    }

    [Fact]
    public async Task CreateTask_ReturnsNotFound_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        _serviceMock
            .Setup(s => s.CreateTaskAsync(projectId, createDto))
            .ThrowsAsync(new ProjectNotFoundException(projectId));

        // Act
        var result = await _controller.CreateTask(projectId, createDto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(projectId.ToString(), notFound.Value!.ToString());
    }

    [Fact]
    public async Task CreateTask_ReturnsConflict_WhenProjectArchived()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        _serviceMock
            .Setup(s => s.CreateTaskAsync(projectId, createDto))
            .ThrowsAsync(new ProjectArchivedException(projectId));

        // Act
        var result = await _controller.CreateTask(projectId, createDto);

        // Assert
        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Contains(projectId.ToString(), conflict.Value!.ToString());
    }

    [Fact]
    public async Task GetTaskById_ReturnsOk_WhenTaskExists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new TaskItemDto
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Task",
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _serviceMock.Setup(s => s.GetTaskByIdAsync(projectId, taskId)).ReturnsAsync(task);

        // Act
        var result = await _controller.GetTaskById(projectId, taskId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(taskId, body.Id);
        Assert.Equal(projectId, body.ProjectId);
    }

    [Fact]
    public async Task GetTaskById_ReturnsNotFound_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetTaskByIdAsync(projectId, taskId))
            .ReturnsAsync((TaskItemDto?)null);

        // Act
        var result = await _controller.GetTaskById(projectId, taskId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllTasksByProjectId_ReturnsOk_WithPopulatedList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new[]
        {
            new TaskItemDto
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Task 1",
                Status = TaskItemStatus.ToDo,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            },
            new TaskItemDto
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "Task 2",
                Status = TaskItemStatus.InProgress,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            },
        };

        _serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId)).ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAllTasksByProjectId(projectId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Equal(2, body.Count());
    }

    [Fact]
    public async Task GetAllTasksByProjectId_ReturnsOk_WithEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetAllTasksByProjectIdAsync(projectId))
            .ReturnsAsync(Enumerable.Empty<TaskItemDto>());

        // Act
        var result = await _controller.GetAllTasksByProjectId(projectId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Empty(body);
    }

    [Fact]
    public async Task GetAllTasksByProjectId_ReturnsNotFound_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetAllTasksByProjectIdAsync(projectId))
            .ThrowsAsync(new ProjectNotFoundException(projectId));

        // Act
        var result = await _controller.GetAllTasksByProjectId(projectId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(projectId.ToString(), notFound.Value!.ToString());
    }

    [Fact]
    public async Task UpdateTask_ReturnsOk_WhenTaskUpdated()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var update = new UpdateTaskDto { Title = "Updated Title", Description = "Updated Desc" };
        var updated = new TaskItemDto
        {
            Id = taskId,
            ProjectId = projectId,
            Title = update.Title,
            Description = update.Description,
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _serviceMock.Setup(s => s.UpdateTaskAsync(projectId, taskId, update)).ReturnsAsync(updated);

        // Act
        var result = await _controller.UpdateTask(projectId, taskId, update);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(taskId, body.Id);
        Assert.Equal(update.Title, body.Title);
        Assert.Equal(update.Description, body.Description);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var update = new UpdateTaskDto { Title = "Title" };

        _serviceMock
            .Setup(s => s.UpdateTaskAsync(projectId, taskId, update))
            .ReturnsAsync((TaskItemDto?)null);

        // Act
        var result = await _controller.UpdateTask(projectId, taskId, update);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
