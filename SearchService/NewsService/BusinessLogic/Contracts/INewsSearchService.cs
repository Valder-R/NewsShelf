
using BusinessLogic.DTO;
using DataAccess.Entities;

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
        /// Performs a combined search using optional filters:
        /// text query, author, date range, category and sorting options.
        /// </summary>
        /// <param name="query">Optional text query (title + content).</param>
        /// <param name="author">Optional author name filter.</param>
        /// <param name="fromDate">Optional start of date range.</param>
        /// <param name="toDate">Optional end of date range.</param>
        /// <param name="category">Optional news category filter.</param>
        /// <param name="sortBy">
        /// Sort field: "title", "author", "category", "date".
        /// Defaults to "date" when null or unknown.
        /// </param>
        /// <param name="sortDescending">True to sort in descending order.</param>
        Task<IReadOnlyList<NewsResponse>> SearchAsync(
            string? query,
            string? author,
            DateTime? fromDate,
            DateTime? toDate,
            NewsCategory? category,
            string? sortBy,
            bool sortDescending);

        /// <summary>
        /// Searches only by text keywords (title + content).
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByTextAsync(
            string query,
            string? sortBy,
            bool sortDescending);

        /// <summary>
        /// Searches only by author name.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByAuthorAsync(
            string author,
            string? sortBy,
            bool sortDescending);

        /// <summary>
        /// Searches only within a specific date range.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByDateRangeAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending);

        /// <summary>
        /// Retrieves all news that belong to a specific category.
        /// </summary>
        Task<IReadOnlyList<NewsResponse>> SearchByCategoryAsync(
            NewsCategory category,
            string? sortBy,
            bool sortDescending); 
    }
}
