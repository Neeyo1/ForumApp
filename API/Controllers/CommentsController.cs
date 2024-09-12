using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class CommentsController(ICommentRepository commentRepository, IUserRepository userRepository,
    ITopicRepository topicRepository, ISectionRepository sectionRepository, IMapper mapper,
    UserManager<AppUser> userManager) : BaseApiController
{
    [HttpGet("{commentId}")]
    public async Task<ActionResult<CommentDto>> GetComment(int commentId)
    {
        var comment = await commentRepository.GetCommentAsync(commentId);
        if (comment == null) return BadRequest("Comment does not exist");

        return Ok(mapper.Map<CommentDto>(comment));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForTopic([FromQuery] int topicId)
    {
        var comments = await commentRepository.GetCommentsAsync(topicId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateTopic(CommentCreateDto commentCreateDto, [FromQuery] int topicId)
    {
        var (user, topic, section) = await GetParentsDataForComment(topicId);
        if (user == null) return BadRequest("User does not exist");
        if (topic == null) return BadRequest("Topic does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot add comments in closed sections");
        if (!topic.IsOpen && !canModerate) return BadRequest("You cannot add comments in closed topics");

        var comment = mapper.Map<Comment>(commentCreateDto);
        comment.TopicId = topicId;
        comment.AuthorId = user.Id;

        commentRepository.AddComment(comment);
        topic.CommentCount++;

        if (await commentRepository.Complete()) return Ok(mapper.Map<CommentDto>(comment));
        return BadRequest("Failed to create comment");
    }

    [HttpPut("{commentId}")]
    public async Task<ActionResult<SectionDto>> EditComment(CommentCreateDto commentCreateDto, int commentId)
    {
        var comment = await commentRepository.GetCommentAsync(commentId);
        if (comment == null) return BadRequest("Comment does not exist");
        if (comment.IsDeleted) return BadRequest("You cannot edit deleted comments");

        var (user, topic, section) = await GetParentsDataForComment(comment.TopicId);
        if (user == null) return BadRequest("User does not exist");
        if (topic == null) return BadRequest("Topic does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot edit comments in closed sections");
        if (!topic.IsOpen && !canModerate) return BadRequest("You cannot edit comments in closed topics");
        if (comment.AuthorId != user.Id && !canModerate) return Unauthorized();

        var commentEdit = new CommentEdit
        {
            Content = comment.Content,
            CommentId = commentId
        };
        commentRepository.AddCommentEdit(commentEdit);

        mapper.Map(commentCreateDto, comment);

        if (await commentRepository.Complete()) return NoContent();
        return BadRequest("Failed to edit comment");
    }

    [HttpDelete("{commentId}")]
    public async Task<ActionResult<SectionDto>> DeleteComment(int commentId)
    {
        var comment = await commentRepository.GetCommentAsync(commentId);
        if (comment == null) return BadRequest("Comment does not exist");

        var (user, topic, section) = await GetParentsDataForComment(comment.TopicId);
        if (user == null) return BadRequest("User does not exist");
        if (topic == null) return BadRequest("Topic does not exist");
        if (section == null) return BadRequest("Section does not exist");

        var userRoles = await userManager.GetRolesAsync(user);
        var canModerate = IsModerator(userRoles);
        if (!section.IsOpen && !canModerate) return BadRequest("You cannot delete comments in closed sections");
        if (!topic.IsOpen && !canModerate) return BadRequest("You cannot delete comments in closed topics");
        if (comment.AuthorId != user.Id && !canModerate) return Unauthorized();

        comment.IsDeleted = true;

        var commentEdit = new CommentEdit
        {
            Content = comment.Content,
            CommentId = commentId
        };
        commentRepository.AddCommentEdit(commentEdit);

        comment.Content = "Comment deleted";

        if (await commentRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete comment");
    }

    private async Task<(AppUser?, Topic?, Section?)> GetParentsDataForComment(int topicId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return (null, null, null);

        var topic = await topicRepository.GetTopicAsync(topicId);
        if (topic == null) return (user, null, null);

        var section = await sectionRepository.GetSectionAsync(topic.SectionId);
        if (section == null) return (user, topic, null);

        return (user, topic, section);
    }

    private static bool IsModerator(IList<string> userRoles)
    {
        return userRoles.Contains("Moderator") || userRoles.Contains("Admin");
    }
}
