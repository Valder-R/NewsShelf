
namespace DataAccess.Entities
{
    public class News
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public string? Author { get; set; }

        public ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
    }
}
