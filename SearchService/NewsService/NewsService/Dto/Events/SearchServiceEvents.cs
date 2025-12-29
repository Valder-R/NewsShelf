namespace NewsService.Dto.Events;



public class NewsSearchedEvent
{
    public required string SearchQuery { get; set; }
    public required string UserId { get; set; }
    public int ResultCount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}



public class NewsViewedEvent
{
    public int NewsId { get; set; }
    public required string UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
