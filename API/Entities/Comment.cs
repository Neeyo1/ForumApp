using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Comments")]
public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    //Comment - Topic
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    //Comment - User
    public int AuthorId { get; set; }
    public AppUser Author { get; set; } = null!;

    //Comment - CommentEdit
    public ICollection<CommentEdit> CommentEdits { get; set; } = [];
}
