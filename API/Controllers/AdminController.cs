using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(DataContext context, IMapper mapper,
    UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireModeratorRole")]
    [HttpGet("comment-edits/{commentId}")]
    public async Task<ActionResult<IEnumerable<CommentEditDto>>> GetCommentEdits(int commentId)
    {
        var commentEdits = await context.CommentEdits
            .Where(x => x.CommentId == commentId)
            .OrderByDescending(x => x.Id)
            .ProjectTo<CommentEditDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(commentEdits);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("No role provided");

        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);
        var results = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!results.Succeeded) return BadRequest("Failed to add to roles");

        results = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!results.Succeeded) return BadRequest("Failed to remove to roles");

        return Ok(await userManager.GetRolesAsync(user));
    }
}
