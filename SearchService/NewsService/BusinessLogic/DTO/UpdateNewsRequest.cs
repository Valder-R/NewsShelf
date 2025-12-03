
using DataAccess.Entities;

namespace BusinessLogic.DTO
{
    public class UpdateNewsRequest
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Author { get; set; }

        public NewsCategory Category { get; set; } = NewsCategory.Other;

        public List<string> ImageUrls { get; set; } = new();
    }
}
