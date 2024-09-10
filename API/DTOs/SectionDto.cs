namespace API.DTOs;

public class SectionDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime Created { get; set; }
    public bool IsOpen { get; set; }
    public int TopicCount { get; set; }
}
