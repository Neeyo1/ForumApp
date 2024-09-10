using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class TopicsController(ITopicRepository topicRepository, ISectionRepository sectionRepository,
    IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpGet("{topicId}")]
    public async Task<ActionResult<TopicDto>> GetTopic(int topicId)
    {
        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        return Ok(mapper.Map<TopicDto>(topic));
    }

    [HttpGet]
    public async Task<ActionResult<TopicDto>> GetTopicsForSection([FromQuery] int sectionId)
    {
        var topics = await topicRepository.GetTopicsAsync(sectionId);
        return Ok(topics);
    }

    [HttpPost]
    public async Task<ActionResult<TopicDto>> CreateTopic(TopicCreateDto topicCreateDto, [FromQuery] int sectionId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User does not exist");

        var section = await sectionRepository.GetSectionAsync(sectionId);
        if (section == null) return BadRequest("Section does not exist");
        if (!section.IsOpen) return BadRequest("You cannot add topic to closed section");

        var topic = mapper.Map<Topic>(topicCreateDto);
        topic.SectionId = sectionId;
        topic.AuthorId = user.Id;

        topicRepository.AddTopic(topic);
        section.TopicCount++;

        if (await topicRepository.Complete()) return Ok(mapper.Map<TopicDto>(topic));
        return BadRequest("Failed to create topic");
    }

    [HttpPut("{topicId}")]
    public async Task<ActionResult<SectionDto>> EditTopic(TopicCreateDto topicCreateDto, int topicId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User does not exist");

        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        if (topic.AuthorId != user.Id) return Unauthorized();

        mapper.Map(topicCreateDto, topic);

        if (await topicRepository.Complete()) return NoContent();
        return BadRequest("Failed to edit topic");
    }

    [HttpDelete("{topicId}")]
    public async Task<ActionResult<SectionDto>> DeleteTopic(int topicId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User does not exist");

        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        if (topic.AuthorId != user.Id) return Unauthorized();

        var section = await sectionRepository.GetSectionAsync(topic.SectionId);
        if (section == null) return BadRequest("Section where topic should belong, does not exist");

        topicRepository.DeleteTopic(topic);
        section.TopicCount--;

        if (await topicRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete topic");
    }
}
