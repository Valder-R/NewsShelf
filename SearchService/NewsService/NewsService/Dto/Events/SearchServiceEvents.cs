namespace NewsService.Dto.Events;

/// <summary>
/// Event published when news is searched
/// </summary>
public class NewsSearchedEvent
{
    public required string SearchQuery { get; set; }
    public required string UserId { get; set; }
    public int ResultCount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when news is viewed
/// </summary>
public class NewsViewedEvent
{
    public int NewsId { get; set; }
    public required string UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
