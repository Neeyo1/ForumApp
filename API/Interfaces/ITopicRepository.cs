using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ITopicRepository
{
    void AddTopic(Topic topic);
    void DeleteTopic(Topic topic);
    Task<Topic?> GetTopicAsync(int topicId);
    Task<PagedList<TopicDto>> GetTopicsAsync(TopicParams topicParams);
    Task<bool> Complete();
}
