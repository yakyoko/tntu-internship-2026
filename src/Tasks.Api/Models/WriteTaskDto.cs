using System.ComponentModel.DataAnnotations;

namespace Tasks.Api.Models;

public abstract class WriteTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Assignee { get; set; }

    public DateTimeOffset? DueDate { get; set; }
}
