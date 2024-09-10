using API.DTOs;
using API.Entities;
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

    public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int sectionId)
    {
        return await context.Topics
            .Where(x => x.SectionId == sectionId)
            .ProjectTo<TopicDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
