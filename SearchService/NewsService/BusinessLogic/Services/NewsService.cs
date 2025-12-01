using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.DTO;
using DataAccess.AppDbContext;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Provides core CRUD business logic for managing news entities.
    /// Responsible for creating, retrieving, updating, and deleting news,
    /// including handling associated image URLs.
    /// </summary>
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageMapper _imageMapper;

        public NewsService(
            ApplicationDbContext context,
            IMapper mapper,
            IImageMapper imageMapper)
        {
            _context = context;
            _mapper = mapper;
            _imageMapper = imageMapper;
        }

        /// <summary>
        /// Retrieves all news entries ordered by publication date (newest first).
        /// </summary>
        /// <returns>
        /// A read-only list of <see cref="NewsResponse"/> objects.
        /// </returns>
        public async Task<IReadOnlyList<NewsResponse>> GetAllAsync()
        {
            var news = await _context.NewsItems
                .Include(n => n.Images)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();

            return _mapper.Map<List<NewsResponse>>(news);
        }

        /// <summary>
        /// Retrieves a single news entry by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the news entry.</param>
        /// <returns>
        /// A <see cref="NewsResponse"/> if found; otherwise, null.
        /// </returns>
        public async Task<NewsResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.NewsItems
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            return entity is null
                ? null
                : _mapper.Map<NewsResponse>(entity);
        }

        /// <summary>
        /// Creates a new news entry using the provided request data.
        /// Image upload should be performed at the API layer;
        /// this service expects ready-to-use image URLs.
        /// </summary>
        /// <param name="request">The request model containing news data.</param>
        /// <returns>
        /// The created <see cref="NewsResponse"/>.
        /// </returns>
        public async Task<NewsResponse> CreateAsync(CreateNewsRequest request)
        {
            // Map basic scalar properties (Title, Content, Author, etc.)
            var entity = _mapper.Map<News>(request);

            entity.PublishedAt = DateTime.UtcNow;

            // Map image URLs → NewsImage entities
            entity.Images = _imageMapper.FromUrls(request.ImageUrls);

            _context.NewsItems.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<NewsResponse>(entity);
        }


        /// <summary>
        /// Updates an existing news entry.
        /// If found, it overwrites text fields and fully replaces associated images.
        /// </summary>
        /// <param name="id">The ID of the news entry to update.</param>
        /// <param name="request">Updated values for the news entry.</param>
        /// <returns>
        /// True if the update succeeded; false if the news entry was not found.
        /// </returns>
        public async Task<bool> UpdateAsync(int id, UpdateNewsRequest request)
        {
            var entity = await _context.NewsItems
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (entity is null)
                return false;

            // Map scalar properties onto existing entity
            _mapper.Map(request, entity);

            // Replace existing images
            _context.NewsImages.RemoveRange(entity.Images);

            entity.Images = _imageMapper.FromUrls(request.ImageUrls, id);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a news entry along with all associated images.
        /// </summary>
        /// <param name="id">The ID of the news entry to delete.</param>
        /// <returns>
        /// True if deletion succeeded; false if the entry does not exist.
        /// </returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.NewsItems
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (entity is null)
                return false;

            _context.NewsImages.RemoveRange(entity.Images);
            _context.NewsItems.Remove(entity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
