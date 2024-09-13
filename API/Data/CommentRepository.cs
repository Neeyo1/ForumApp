using API.DTOs;
using API.Entities;
using API.Helpers;
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

    public async Task<PagedList<CommentDto>> GetCommentsAsync(CommentParams commentParams)
    {
        var query = context.Comments.AsQueryable();
        query = query.Where(x => x.TopicId == commentParams.TopicId);

        query = commentParams.OrderBy switch
        {
            "created" => query.OrderBy(x => x.Id),
            "createdDesc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await PagedList<CommentDto>.CreateAsync(
            query.ProjectTo<CommentDto>(mapper.ConfigurationProvider), 
            commentParams.PageNumber, commentParams.PageSize);
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
