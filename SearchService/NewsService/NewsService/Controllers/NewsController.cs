using BusinessLogic.Contracts;
using BusinessLogic.DTO;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using NewsApi.Dto;


namespace NewsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly INewsSearchService _newsSearchService;
        private readonly IWebHostEnvironment _env;

        public NewsController(
            INewsService newsService,
            INewsSearchService newsSearchService,
            IWebHostEnvironment env)
        {
            _newsService = newsService;
            _newsSearchService = newsSearchService;
            _env = env;
        }

        // ==========================
        // CRUD
        // ==========================

        /// <summary>
        /// Returns all news entries ordered by publication date (newest first).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> GetAll()
        {
            var items = await _newsService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Returns a single news entry by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<NewsResponse>> GetById(int id)
        {
            var item = await _newsService.GetByIdAsync(id);
            if (item is null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Creates a news entry and uploads image files
        /// (user selects photos from device).
        /// Files are stored under wwwroot/uploads/news and exposed as URLs.
        /// </summary>
        [HttpPost]
        [RequestSizeLimit(20_000_000)] // 20 MB
        public async Task<ActionResult<NewsResponse>> Create([FromForm] CreateNewsWithFilesDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is empty.");

            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Title and Content are required.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var uploadRoot = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads",
                "news");

            Directory.CreateDirectory(uploadRoot);

            var imageUrls = new List<string>();

            foreach (var file in dto.Images)
            {
                if (file == null || file.Length == 0)
                    continue;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    return BadRequest(new
                    {
                        error = $"File type {ext} is not allowed. Only .jpg, .jpeg and .png are supported."
                    });
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadRoot, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var url = $"{baseUrl}/uploads/news/{fileName}";
                imageUrls.Add(url);
            }

            var request = new CreateNewsRequest
            {
                Title = dto.Title,
                Content = dto.Content,
                Author = dto.Author,
                Category = dto.Category,
                ImageUrls = imageUrls
            };

            var created = await _newsService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing news entry.
        /// Expects image URLs which will fully replace existing images.
        /// (можеш потім зробити окремий PUT з файлами, якщо треба).
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNewsRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var success = await _newsService.UpdateAsync(id, request);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes an existing news entry and its related images.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _newsService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = Enum.GetValues(typeof(NewsCategory))
                .Cast<NewsCategory>()
                .Select(c => new CategoryDto
                {
                    Name = c.ToString(),
                    Value = (int)c
                })
                .ToList();

            return Ok(categories);
        }

        // Search endpoints

        /// <summary>
        /// Combined search: text, author, date range, category, sorting.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> Search(
            [FromQuery] string? query,
            [FromQuery] string? author,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] NewsCategory? category,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = true)
        {
            var result = await _newsSearchService.SearchAsync(
                query,
                author,
                fromDate,
                toDate,
                category,
                sortBy,
                sortDesc);

            return Ok(result);
        }

        /// <summary>
        /// Searches news only by text (title + content).
        /// </summary>
        [HttpGet("search/by-text")]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> SearchByText(
            [FromQuery] string query,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = true)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            var result = await _newsSearchService.SearchByTextAsync(query, sortBy, sortDesc);
            return Ok(result);
        }

        /// <summary>
        /// Searches news only by author.
        /// </summary>
        [HttpGet("search/by-author")]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> SearchByAuthor(
            [FromQuery] string author,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = true)
        {
            if (string.IsNullOrWhiteSpace(author))
                return BadRequest("Author parameter is required.");

            var result = await _newsSearchService.SearchByAuthorAsync(author, sortBy, sortDesc);
            return Ok(result);
        }

        /// <summary>
        /// Searches news only within a specific date range.
        /// </summary>
        [HttpGet("search/by-date-range")]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> SearchByDateRange(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = true)
        {
            var result = await _newsSearchService.SearchByDateRangeAsync(fromDate, toDate, sortBy, sortDesc);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all news that belong to a specific category.
        /// </summary>
        [HttpGet("search/by-category")]
        public async Task<ActionResult<IReadOnlyList<NewsResponse>>> SearchByCategory(
            [FromQuery] NewsCategory category,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = true)
        {
            var result = await _newsSearchService.SearchByCategoryAsync(category, sortBy, sortDesc);
            return Ok(result);
        }
    }
}
