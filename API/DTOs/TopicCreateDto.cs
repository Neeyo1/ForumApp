using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class TopicCreateDto
{
    [Required] public required string Name { get; set; }
}
