using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Topics")]
public class Topic
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public bool IsOpen { get; set; } = true;
    public int CommentCount { get; set; }

    //Topic - Section
    public int SectionId { get; set; }
    public Section Section { get; set; } = null!;

    //Topic - Comment
    public ICollection<Comment> Comments { get; set; } = [];

    //Topic - User
    public int AuthorId { get; set; }
    public AppUser Author { get; set; } = null!;
}
