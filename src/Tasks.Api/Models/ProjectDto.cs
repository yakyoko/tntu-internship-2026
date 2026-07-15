namespace Tasks.Api.Models;

public class ProjectDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsArchived { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
