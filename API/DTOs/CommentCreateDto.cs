using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CommentCreateDto
{
    [Required] public required string Content { get; set; }
}
