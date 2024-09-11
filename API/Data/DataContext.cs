using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<CommentEdit> CommentEdits { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //AppUser - AppRole
        builder.Entity<AppUser>()
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        //Section - Topic
        builder.Entity<Section>()
            .HasMany(x => x.Topics)
            .WithOne(x => x.Section)
            .HasForeignKey(x => x.SectionId)
            .IsRequired();

        //Topic - Comment
        builder.Entity<Topic>()
            .HasMany(x => x.Comments)
            .WithOne(x => x.Topic)
            .HasForeignKey(x => x.TopicId)
            .IsRequired();

        //User - Topic
        builder.Entity<AppUser>()
            .HasMany(x => x.AuthorOfTopics)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired();

        //User - Comment
        builder.Entity<AppUser>()
            .HasMany(x => x.AuthorOfComments)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired();

        //Comment - CommentEdit
        builder.Entity<Comment>()
            .HasMany(x => x.CommentEdits)
            .WithOne(x => x.Comment)
            .HasForeignKey(x => x.CommentId)
            .IsRequired();
    }
}
