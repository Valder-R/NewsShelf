using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.DTO;
using DataAccess.AppDbContext;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Implements various search operations for news, including:
    /// - text search (title + content)
    /// - author-based search
    /// - date range filtering
    /// - combined search using multiple filters and sorting
    /// </summary>
    public class NewsSearchService : INewsSearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public NewsSearchService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Performs a full combined search using optional text query, author filter, 
        /// date range, and sorting options.
        /// </summary>
        public async Task<IReadOnlyList<NewsResponse>> SearchAsync(
            string? query,
            string? author,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending)
        {
            var newsQuery = BaseQuery();

            newsQuery = ApplyTextFilter(newsQuery, query);
            newsQuery = ApplyAuthorFilter(newsQuery, author);
            newsQuery = ApplyDateFilter(newsQuery, fromDate, toDate);
            newsQuery = ApplySorting(newsQuery, sortBy, sortDescending);

            return await ExecuteAsync(newsQuery);
        }

        /// <summary>
        /// Searches only by text keywords (title + content).
        /// </summary>
        public async Task<IReadOnlyList<NewsResponse>> SearchByTextAsync(
            string query,
            string? sortBy,
            bool sortDescending)
        {
            var newsQuery = BaseQuery();

            newsQuery = ApplyTextFilter(newsQuery, query);
            newsQuery = ApplySorting(newsQuery, sortBy, sortDescending);

            return await ExecuteAsync(newsQuery);
        }

        /// <summary>
        /// Searches only by author name.
        /// </summary>
        public async Task<IReadOnlyList<NewsResponse>> SearchByAuthorAsync(
            string author,
            string? sortBy,
            bool sortDescending)
        {
            var newsQuery = BaseQuery();

            newsQuery = ApplyAuthorFilter(newsQuery, author);
            newsQuery = ApplySorting(newsQuery, sortBy, sortDescending);

            return await ExecuteAsync(newsQuery);
        }

        /// <summary>
        /// Searches only within a specific date range.
        /// </summary>
        public async Task<IReadOnlyList<NewsResponse>> SearchByDateRangeAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? sortBy,
            bool sortDescending)
        {
            var newsQuery = BaseQuery();

            newsQuery = ApplyDateFilter(newsQuery, fromDate, toDate);
            newsQuery = ApplySorting(newsQuery, sortBy, sortDescending);

            return await ExecuteAsync(newsQuery);
        }

        // ===========================
        // Helper Methods
        // ===========================

        private IQueryable<News> BaseQuery()
        {
            return _context.NewsItems
                .Include(n => n.Images)
                .AsQueryable();
        }

        private static IQueryable<News> ApplyTextFilter(IQueryable<News> query, string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return query;

            var q = text.Trim();

            return query.Where(n =>
                n.Title.Contains(q) ||
                n.Content.Contains(q));
        }

        private static IQueryable<News> ApplyAuthorFilter(IQueryable<News> query, string? author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return query;

            var a = author.Trim();

            return query.Where(n => n.Author != null && n.Author.Contains(a));
        }

        private static IQueryable<News> ApplyDateFilter(
            IQueryable<News> query,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (fromDate.HasValue)
            {
                query = query.Where(n => n.PublishedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(n => n.PublishedAt <= toDate.Value);
            }

            return query;
        }

        private static IQueryable<News> ApplySorting(
            IQueryable<News> query,
            string? sortBy,
            bool sortDescending)
        {
            var sort = (sortBy ?? "date").ToLowerInvariant();

            return (sort, sortDescending) switch
            {
                ("title", false) => query.OrderBy(n => n.Title),
                ("title", true) => query.OrderByDescending(n => n.Title),

                ("author", false) => query.OrderBy(n => n.Author),
                ("author", true) => query.OrderByDescending(n => n.Author),

                ("date", false) => query.OrderBy(n => n.PublishedAt),
                ("date", true) => query.OrderByDescending(n => n.PublishedAt),

                _ => query.OrderByDescending(n => n.PublishedAt)
            };
        }

        private async Task<IReadOnlyList<NewsResponse>> ExecuteAsync(IQueryable<News> query)
        {
            var items = await query.ToListAsync();
            return _mapper.Map<List<NewsResponse>>(items);
        }
    }
}
