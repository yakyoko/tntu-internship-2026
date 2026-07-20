using System.ComponentModel.DataAnnotations;

namespace Tasks.Api.Models;

public abstract class WriteTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public required string Title { get; set; }

    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    [StringLength(200, ErrorMessage = "Assignee must not exceed 200 characters.")]
    public string? Assignee { get; set; }

    public DateTimeOffset? DueDate { get; set; }
}
