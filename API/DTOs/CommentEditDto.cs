namespace API.DTOs;

public class CommentEditDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime Edited { get; set; }
}
