using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ICommentRepository
{
    void AddComment(Comment comment);
    void AddCommentEdit(CommentEdit commentEdit);
    Task<Comment?> GetCommentAsync(int commentId);
    Task<PagedList<CommentDto>> GetCommentsAsync(CommentParams commentParams);
    Task<bool> Complete();
}
