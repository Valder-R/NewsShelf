
namespace BusinessLogic.DTO
{
    public class NewsResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime PublishedAt { get; set; }

        public string? Author { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
