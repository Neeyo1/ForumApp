using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class CommentRepository(DataContext context, IMapper mapper) : ICommentRepository
{
    public void AddComment(Comment comment)
    {
        context.Comments.Add(comment);
    }

    public void AddCommentEdit(CommentEdit commentEdit)
    {
        context.CommentEdits.Add(commentEdit);
    }

    public async Task<Comment?> GetCommentAsync(int commentId)
    {
        return await context.Comments
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(int topicId)
    {
        return await context.Comments
            .Where(x => x.TopicId == topicId)
            .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
