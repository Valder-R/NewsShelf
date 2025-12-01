using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> NewsItems => Set<News>();
        public DbSet<NewsImage> NewsImages => Set<NewsImage>();
    }
}
