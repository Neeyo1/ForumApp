using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ICommentRepository
{
    void AddComment(Comment comment);
    void AddCommentEdit(CommentEdit commentEdit);
    Task<Comment?> GetCommentAsync(int commentId);
    Task<IEnumerable<CommentDto>> GetCommentsAsync(int topicId);
    Task<bool> Complete();
}
