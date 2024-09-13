namespace API.Helpers;

public class TopicParams : PaginationParams
{
    public string? Status { get; set; }
    public string OrderBy { get; set; } = "activeDesc";
    public int SectionId { get; set; }
}