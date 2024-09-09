using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    //User - Topic
    public ICollection<Topic> AuthorOfTopics { get; set; } = [];

    //User - Comment
    public ICollection<Comment> AuthorOfComments { get; set; } = [];
}
