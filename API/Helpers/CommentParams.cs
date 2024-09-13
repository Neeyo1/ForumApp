namespace API.Helpers;

public class CommentParams : PaginationParams
{
    public string OrderBy { get; set; } = "createDesc";
    public int TopicId { get; set; }
}
