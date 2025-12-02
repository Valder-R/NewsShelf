namespace NewsApi.Dto
{
    public class CreateNewsWithFilesDto
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Author { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }
}
