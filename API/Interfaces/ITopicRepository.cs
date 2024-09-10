using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ITopicRepository
{
    void AddTopic(Topic topic);
    void DeleteTopic(Topic topic);
    Task<Topic?> GetTopicAsync(int topicId);
    Task<IEnumerable<TopicDto>> GetTopicsAsync(int sectionId);
    Task<bool> Complete();
}
