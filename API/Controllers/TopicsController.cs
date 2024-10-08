using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class TopicsController(ITopicRepository topicRepository, ISectionRepository sectionRepository,
    IUserRepository userRepository, UserManager<AppUser> userManager, IMapper mapper) : BaseApiController
{
    [HttpGet("{topicId}")]
    public async Task<ActionResult<TopicDto>> GetTopic(int topicId)
    {
        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        return Ok(mapper.Map<TopicDto>(topic));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopicsForSection([FromQuery] TopicParams topicParams)
    {
        var topics = await topicRepository.GetTopicsAsync(topicParams);
        Response.AddPaginationHeader(topics);
        return Ok(topics);
    }

    [HttpPost]
    public async Task<ActionResult<TopicDto>> CreateTopic(TopicCreateDto topicCreateDto, [FromQuery] int sectionId)
    {
        var (user, section) = await GetParentsDataForTopic(sectionId);
        if (user == null) return BadRequest("User does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot add topic to closed section");

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
        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        var (user, section) = await GetParentsDataForTopic(topic.SectionId);
        if (user == null) return BadRequest("User does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot edit topics in closed sections");
        if (topic.AuthorId != user.Id && !canModerate) return Unauthorized();

        mapper.Map(topicCreateDto, topic);

        if (await topicRepository.Complete()) return NoContent();
        return BadRequest("Failed to edit topic");
    }

    [HttpDelete("{topicId}")]
    public async Task<ActionResult<SectionDto>> DeleteTopic(int topicId)
    {
        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return BadRequest("Topic does not exist");

        var (user, section) = await GetParentsDataForTopic(topic.SectionId);
        if (user == null) return BadRequest("User does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot delete topics in closed sections");
        if (topic.AuthorId != user.Id && !canModerate) return Unauthorized();

        topicRepository.DeleteTopic(topic);
        section.TopicCount--;

        if (await topicRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete topic");
    }

    private async Task<(AppUser?, Section?)> GetParentsDataForTopic(int sectionId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return (null, null);

        var section = await sectionRepository.GetSectionAsync(sectionId);
        if (section == null) return (user, null);

        return (user, section);
    }

    private static bool IsModerator(IList<string> userRoles)
    {
        return userRoles.Contains("Moderator") || userRoles.Contains("Admin");
    }
}
