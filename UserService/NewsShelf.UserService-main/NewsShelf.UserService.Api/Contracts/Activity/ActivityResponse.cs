using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Contracts.Activity;

public class ActivityResponse
{
    public Guid Id { get; set; }
    public string NewsId { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string? Topic { get; set; }
    public ActivityType ActivityType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
