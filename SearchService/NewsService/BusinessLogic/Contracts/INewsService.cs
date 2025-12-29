
using BusinessLogic.DTO;

namespace BusinessLogic.Contracts
{





    public interface INewsService
    {




        Task<IReadOnlyList<NewsResponse>> GetAllAsync();







        Task<NewsResponse?> GetByIdAsync(int id);






        Task<NewsResponse> CreateAsync(CreateNewsRequest request);








        Task<bool> UpdateAsync(int id, UpdateNewsRequest request);







        Task<bool> DeleteAsync(int id);
    }
}
