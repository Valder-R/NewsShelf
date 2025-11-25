namespace NewsShelf.UserService.Api.Models;

public class UserActivity
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string NewsId { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string? Topic { get; set; }
    public ActivityType ActivityType { get; set; }
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public ApplicationUser? User { get; set; }
}
