using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mappers
{
    public class ImageMapper : IImageMapper
    {
        public List<NewsImage> FromUrls(IEnumerable<string> urls, int? newsId = null)
        {
            return urls
                .Select(url => new NewsImage
                {
                    Url = url,
                    NewsId = newsId.HasValue ? newsId.Value : 0 // will be set by EF for Create
                })
                .ToList();
        }
    }
}
