
using BusinessLogic.DTO;

namespace BusinessLogic.Contracts
{
    /// <summary>
    /// Defines CRUD operations for managing news entities.
    /// Provides methods for retrieving, creating, updating,
    /// and deleting news within the system.
    /// </summary>
    public interface INewsService
    {
        /// <summary>
        /// Retrieves all news entries ordered by publication date.
        /// </summary>
        /// <returns>A read-only list of news response objects.</returns>
        Task<IReadOnlyList<NewsResponse>> GetAllAsync();

        /// <summary>
        /// Retrieves a single news entry by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the news entry.</param>
        /// <returns>
        /// A <see cref="NewsResponse"/> if found; otherwise, null.
        /// </returns>
        Task<NewsResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new news entry using the provided request data.
        /// Image URLs should already be processed at the API layer.
        /// </summary>
        /// <param name="request">Data required to create a news entry.</param>
        /// <returns>The created news entry as a <see cref="NewsResponse"/>.</returns>
        Task<NewsResponse> CreateAsync(CreateNewsRequest request);

        /// <summary>
        /// Updates an existing news entry.
        /// </summary>
        /// <param name="id">The ID of the news entry to update.</param>
        /// <param name="request">Updated values for the news entry.</param>
        /// <returns>
        /// True if the update was successful; false if the news entry was not found.
        /// </returns>
        Task<bool> UpdateAsync(int id, UpdateNewsRequest request);

        /// <summary>
        /// Deletes a news entry along with its associated images.
        /// </summary>
        /// <param name="id">The ID of the news entry to delete.</param>
        /// <returns>
        /// True if deletion succeeded; false if the news entry was not found.
        /// </returns>
        Task<bool> DeleteAsync(int id);
    }
}
