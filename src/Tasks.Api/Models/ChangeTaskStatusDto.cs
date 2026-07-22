using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tasks.Api.Models;

public class ChangeTaskStatusDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    [Required]
    public TaskItemStatus Status { get; set; }
}
