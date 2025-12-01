
using DataAccess.Entities;

namespace BusinessLogic.Contracts
{
    public interface IImageMapper
    {
        List<NewsImage> FromUrls(IEnumerable<string> urls, int? newsId = null);
    }
}
