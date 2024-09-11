using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("CommentEdits")]
public class CommentEdit
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime Edited { get; set; } = DateTime.UtcNow;

    //CommentEdit - Comment
    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
}
