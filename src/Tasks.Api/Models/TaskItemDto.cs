namespace Tasks.Api.Models;

public class TaskItemDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; }

    public string? Assignee { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
