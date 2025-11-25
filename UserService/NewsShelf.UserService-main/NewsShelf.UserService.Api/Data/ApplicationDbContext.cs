using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<UserFavoriteTopic> UserFavoriteTopics => Set<UserFavoriteTopic>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(128);
            entity.Property(u => u.Bio).HasMaxLength(1024);
        });

        builder.Entity<UserFavoriteTopic>(entity =>
        {
            entity.HasIndex(t => new { t.UserId, t.Topic }).IsUnique();
            entity.Property(t => t.Topic).HasMaxLength(64);
        });

        builder.Entity<UserActivity>(entity =>
        {
            entity.Property(a => a.NewsId).HasMaxLength(128);
            entity.Property(a => a.NewsTitle).HasMaxLength(256);
            entity.Property(a => a.Topic).HasMaxLength(64);
        });
    }
}
