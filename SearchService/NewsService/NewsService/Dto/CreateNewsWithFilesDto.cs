using DataAccess.Entities;

namespace NewsApi.Dto
{
    public class CreateNewsWithFilesDto
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Author { get; set; }

        public NewsCategory Category { get; set; } = NewsCategory.Other;

        public List<IFormFile> Images { get; set; } = new();
    }
}
