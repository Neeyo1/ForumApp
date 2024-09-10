namespace API.DTOs;

public class TopicDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime LastActive { get; set; }
    public bool IsOpen { get; set; }
    public int CommentCount { get; set; }
}
