
using BusinessLogic.DTO;
using DataAccess.Entities;

namespace BusinessLogic.Contracts
{





    public interface INewsSearchService
    {














        Task<IReadOnlyList<NewsResponse>> SearchAsync(
            string? query,
            string? author,
            DateTime? fromDate,
            DateTime? toDate,
            NewsCategory? category,
            string? sortBy,
            bool sortDescending);



        Task<IReadOnlyList<NewsResponse>> SearchByTextAsync(
            string query,
            string? sortBy,
            bool sortDescending);



        Task<IReadOnlyList<NewsResponse>> SearchByAuthorAsync(
            string author,
            string? sortBy,
            bool sortDescending);



        Task<IReadOnlyList<NewsResponse>> SearchByDateRangeAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending);



        Task<IReadOnlyList<NewsResponse>> SearchByCategoryAsync(
            NewsCategory category,
            string? sortBy,
            bool sortDescending); 
    }
}
