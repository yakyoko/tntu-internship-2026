using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tasks.Api.Controllers;
using Tasks.Api.Exceptions;
using Tasks.Api.Interfaces;
using Tasks.Api.Models;

namespace Tasks.Api.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock = new();
    private readonly Mock<ILogger<TasksController>> _loggerMock = new();
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        this._controller = new TasksController(this._serviceMock.Object, this._loggerMock.Object);
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

        this._serviceMock.Setup(s => s.CreateTaskAsync(projectId, createDto)).ReturnsAsync(created);

        // Act
        var result = await this._controller.CreateTask(projectId, createDto);

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
    public async Task CreateTask_ThrowsProjectNotFoundException_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        this._serviceMock.Setup(s => s.CreateTaskAsync(projectId, createDto))
            .ThrowsAsync(new ProjectNotFoundException(projectId));

        // Act & Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            this._controller.CreateTask(projectId, createDto)
        );
    }

    [Fact]
    public async Task CreateTask_ThrowsProjectArchivedException_WhenProjectArchived()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        this._serviceMock.Setup(s => s.CreateTaskAsync(projectId, createDto))
            .ThrowsAsync(new ProjectArchivedException(projectId));

        // Act & Assert
        await Assert.ThrowsAsync<ProjectArchivedException>(() =>
            this._controller.CreateTask(projectId, createDto)
        );
    }

    [Fact]
    public async Task CreateTask_ThrowsProjectApiUnavailableException_WhenProjectsApiDown()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var createDto = new CreateTaskDto { Title = "Task" };

        this._serviceMock.Setup(s => s.CreateTaskAsync(projectId, createDto))
            .ThrowsAsync(new ProjectApiUnavailableException(new Exception("timeout")));

        // Act & Assert
        await Assert.ThrowsAsync<ProjectApiUnavailableException>(() =>
            this._controller.CreateTask(projectId, createDto)
        );
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

        this._serviceMock.Setup(s => s.GetTaskByIdAsync(projectId, taskId)).ReturnsAsync(task);

        // Act
        var result = await this._controller.GetTaskById(projectId, taskId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(taskId, body.Id);
        Assert.Equal(projectId, body.ProjectId);
    }

    [Fact]
    public async Task GetTaskById_ThrowsTaskNotFoundException_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        this._serviceMock.Setup(s => s.GetTaskByIdAsync(projectId, taskId))
            .ThrowsAsync(new TaskNotFoundException(taskId));

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            this._controller.GetTaskById(projectId, taskId)
        );
    }

    [Fact]
    public async Task GetTasksByProjectId_ReturnsOk_WithPopulatedList()
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
        this._serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId, null))
            .ReturnsAsync(tasks);

        // Act
        var result = await this._controller.GetAllTasksByProjectId(projectId, null);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Equal(2, body.Count());
    }

    [Fact]
    public async Task GetTasksByProjectId_ReturnsOk_WithEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        this._serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId, null))
            .ReturnsAsync(Enumerable.Empty<TaskItemDto>());

        // Act
        var result = await this._controller.GetAllTasksByProjectId(projectId, null);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Empty(body);
    }

    [Fact]
    public async Task GetTasksByProjectId_ThrowsProjectNotFoundException_WhenProjectMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        this._serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId, null))
            .ThrowsAsync(new ProjectNotFoundException(projectId));

        // Act & Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            this._controller.GetAllTasksByProjectId(projectId, null)
        );
    }

    [Fact]
    public async Task GetTasksByProjectId_ThrowsProjectApiUnavailableException_WhenProjectsApiDown()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        this._serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId, null))
            .ThrowsAsync(new ProjectApiUnavailableException(new Exception("timeout")));

        // Act & Assert
        await Assert.ThrowsAsync<ProjectApiUnavailableException>(() =>
            this._controller.GetAllTasksByProjectId(projectId, null)
        );
    }

    [Fact]
    public async Task GetTasksByProjectId_ReturnsOk_WithOnlyMatchingStatus_WhenFilterApplied()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var inProgressTasks = new[]
        {
            new TaskItemDto
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = "In progress task",
                Status = TaskItemStatus.InProgress,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            },
        };
        this._serviceMock.Setup(s =>
                s.GetAllTasksByProjectIdAsync(projectId, TaskItemStatus.InProgress)
            )
            .ReturnsAsync(inProgressTasks);

        // Act
        var result = await this._controller.GetAllTasksByProjectId(
            projectId,
            new TaskFilterDto() { Status = TaskItemStatus.InProgress }
        );

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        var bodyList = body.ToList();
        Assert.Single(bodyList);
        Assert.All(bodyList, dto => Assert.Equal(TaskItemStatus.InProgress, dto.Status));

        // Confirms the controller passes the query-bound status through untouched
        this._serviceMock.Verify(
            s => s.GetAllTasksByProjectIdAsync(projectId, TaskItemStatus.InProgress),
            Times.Once
        );
    }

    [Fact]
    public async Task GetTasksByProjectId_ReturnsOk_WithEmptyList_WhenNoTasksMatchStatusFilter()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        this._serviceMock.Setup(s => s.GetAllTasksByProjectIdAsync(projectId, TaskItemStatus.Done))
            .ReturnsAsync(Enumerable.Empty<TaskItemDto>());

        // Act
        var result = await this._controller.GetAllTasksByProjectId(
            projectId,
            new TaskFilterDto() { Status = TaskItemStatus.Done }
        );

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Empty(body);
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

        this._serviceMock.Setup(s => s.UpdateTaskAsync(projectId, taskId, update))
            .ReturnsAsync(updated);

        // Act
        var result = await this._controller.UpdateTask(projectId, taskId, update);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(taskId, body.Id);
        Assert.Equal(update.Title, body.Title);
        Assert.Equal(update.Description, body.Description);
    }

    [Fact]
    public async Task UpdateTask_ThrowsTaskNotFoundException_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var update = new UpdateTaskDto { Title = "Title" };

        this._serviceMock.Setup(s => s.UpdateTaskAsync(projectId, taskId, update))
            .ThrowsAsync(new TaskNotFoundException(taskId));

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            this._controller.UpdateTask(projectId, taskId, update)
        );
    }

    [Fact]
    public async Task ChangeTaskStatus_ReturnsOk_WhenTransitionSucceeds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new ChangeTaskStatusDto { Status = TaskItemStatus.InProgress };
        var updated = new TaskItemDto
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Task",
            Status = TaskItemStatus.InProgress,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        this._serviceMock.Setup(s => s.ChangeTaskStatusAsync(projectId, taskId, request))
            .ReturnsAsync(updated);

        // Act
        var result = await this._controller.ChangeTaskStatus(projectId, taskId, request);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(TaskItemStatus.InProgress, body.Status);
    }

    [Fact]
    public async Task ChangeTaskStatus_ThrowsTaskNotFoundException_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new ChangeTaskStatusDto { Status = TaskItemStatus.InProgress };

        this._serviceMock.Setup(s => s.ChangeTaskStatusAsync(projectId, taskId, request))
            .ThrowsAsync(new TaskNotFoundException(taskId));

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            this._controller.ChangeTaskStatus(projectId, taskId, request)
        );
    }

    [Fact]
    public async Task ChangeTaskStatus_ThrowsInvalidTaskStatusTransitionException_WhenTransitionInvalid()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new ChangeTaskStatusDto { Status = TaskItemStatus.Done };

        this._serviceMock.Setup(s => s.ChangeTaskStatusAsync(projectId, taskId, request))
            .ThrowsAsync(
                new InvalidTaskStatusTransitionException(TaskItemStatus.ToDo, TaskItemStatus.Done)
            );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTaskStatusTransitionException>(() =>
            this._controller.ChangeTaskStatus(projectId, taskId, request)
        );
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent_WhenTaskDeleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        this._serviceMock.Setup(s => s.DeleteTaskAsync(projectId, taskId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await this._controller.DeleteTask(projectId, taskId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        this._serviceMock.Verify(s => s.DeleteTaskAsync(projectId, taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ThrowsTaskNotFoundException_WhenTaskMissing()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        this._serviceMock.Setup(s => s.DeleteTaskAsync(projectId, taskId))
            .ThrowsAsync(new TaskNotFoundException(taskId));

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            this._controller.DeleteTask(projectId, taskId)
        );
    }
}
