namespace Tasks.Api.Models;

public class TaskItem
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Status { get; set; }

    public string? Assignee { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
