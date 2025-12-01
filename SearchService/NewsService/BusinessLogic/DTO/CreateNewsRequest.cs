
namespace BusinessLogic.DTO
{
    public class CreateNewsRequest
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Author { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
