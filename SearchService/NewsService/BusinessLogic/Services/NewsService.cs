using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.DTO;
using DataAccess.AppDbContext;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{





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






        public async Task<IReadOnlyList<NewsResponse>> GetAllAsync()
        {
            var news = await _context.NewsItems
                .Include(n => n.Images)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();

            return _mapper.Map<List<NewsResponse>>(news);
        }







        public async Task<NewsResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.NewsItems
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            return entity is null
                ? null
                : _mapper.Map<NewsResponse>(entity);
        }









        public async Task<NewsResponse> CreateAsync(CreateNewsRequest request)
        {

            var entity = _mapper.Map<News>(request);

            entity.PublishedAt = DateTime.UtcNow;

            entity.Images = _imageMapper.FromUrls(request.ImageUrls);

            _context.NewsItems.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<NewsResponse>(entity);
        }









        public async Task<bool> UpdateAsync(int id, UpdateNewsRequest request)
        {
            var entity = await _context.NewsItems
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (entity is null)
                return false;

            _mapper.Map(request, entity);

            _context.NewsImages.RemoveRange(entity.Images);

            entity.Images = _imageMapper.FromUrls(request.ImageUrls, id);

            await _context.SaveChangesAsync();
            return true;
        }







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
