using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class TopicRepository(DataContext context, IMapper mapper) : ITopicRepository
{
    public void AddTopic(Topic topic)
    {
        context.Topics.Add(topic);
    }

    public void DeleteTopic(Topic topic)
    {
        context.Topics.Remove(topic);
    }

    public async Task<Topic?> GetTopicAsync(int topicId)
    {
        return await context.Topics
            .FirstOrDefaultAsync(x => x.Id == topicId);
    }

    public async Task<PagedList<TopicDto>> GetTopicsAsync(TopicParams topicParams)
    {
        var query = context.Topics.AsQueryable();
        query = query.Where(x => x.SectionId == topicParams.SectionId);

        if (topicParams.Status != null)
        {
            if (topicParams.Status == "open") query = query.Where(x => x.IsOpen == true);
            if (topicParams.Status == "close") query = query.Where(x => x.IsOpen == false);
        }

        query = topicParams.OrderBy switch
        {
            "created" => query.OrderBy(x => x.Id),
            "createdDesc" => query.OrderByDescending(x => x.Id),
            "active" => query.OrderBy(x => x.LastActive),
            "activeDesc" => query.OrderByDescending(x => x.LastActive),
            "comments" => query.OrderBy(x => x.CommentCount),
            "commentsDesc" => query.OrderByDescending(x => x.CommentCount),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<TopicDto>.CreateAsync(
            query.ProjectTo<TopicDto>(mapper.ConfigurationProvider), 
            topicParams.PageNumber, topicParams.PageSize);
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
