
using BusinessLogic.DTO;

namespace BusinessLogic.Contracts
{
    /// <summary>
    /// Provides multiple search operations for news, 
    /// including full combined search, text-only search, 
    /// author-based search, and date-range filtering.
    /// </summary>
    public interface INewsSearchService
    {
        /// <summary>
        /// Performs a combined search using optional filters such as text query, 
        /// author name, date range, and sorting options.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchAsync(
            string? query,
            string? author,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending
        );

        /// <summary>
        /// Searches for news using text keywords in both the title and content.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByTextAsync(
            string query,
            string? sortBy,
            bool sortDescending
        );

        /// <summary>
        /// Searches for news written by a specific author.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByAuthorAsync(
            string author,
            string? sortBy,
            bool sortDescending
        );

        /// <summary>
        /// Searches for news published within a specified date range.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByDateRangeAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending
        );
    }
}
