namespace NewsShelf.UserService.Api.Contracts.Events;

/// <summary>
/// Event published when user registers
/// </summary>
public class UserRegisteredEvent
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when user adds favorite topics
/// </summary>
public class FavoriteTopicAddedEvent
{
    public required string UserId { get; set; }
    public List<string> Topics { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when user reads a news article
/// </summary>
public class NewsReadEvent
{
    public required string UserId { get; set; }
    public required string NewsId { get; set; }
    public required string Category { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when user updates profile
/// </summary>
public class UserProfileUpdatedEvent
{
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    public List<string> FavoriteTopics { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
