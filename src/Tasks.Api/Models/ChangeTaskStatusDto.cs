using System.ComponentModel.DataAnnotations;

namespace Tasks.Api.Models;

public class ChangeTaskStatusDto
{
    [Required]
    public TaskItemStatus Status { get; set; }
}
