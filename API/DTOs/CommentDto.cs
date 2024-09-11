namespace API.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public int AuthorId { get; set; }
    public DateTime Created { get; set; }
    public bool IsDeleted { get; set; }
}
