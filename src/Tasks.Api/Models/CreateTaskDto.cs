namespace Tasks.Api.Models;

public class CreateTaskDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Assignee { get; set; }

    public DateTimeOffset? DueDate { get; set; }
}
