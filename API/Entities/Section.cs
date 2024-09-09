using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Sections")]
public class Section
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsOpen { get; set; } = true;
    public int TopicCount { get; set; }
    
    //Section - Topic
    public ICollection<Topic> Topics { get; set; } = [];
}
