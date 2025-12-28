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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store enum Category as string
            modelBuilder.Entity<News>()
                .Property(n => n.Category)
                .HasConversion<string>();
        }
    }
}
