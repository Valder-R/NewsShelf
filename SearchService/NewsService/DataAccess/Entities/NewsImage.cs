
namespace DataAccess.Entities
{
    public class NewsImage
    {
        public int Id { get; set; }

        public string Url { get; set; } = null!; 

        public int NewsId { get; set; }

        public News News { get; set; } = null!;
    }
}
