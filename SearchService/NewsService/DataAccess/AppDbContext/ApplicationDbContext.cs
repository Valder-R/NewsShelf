using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        Database.EnsureCreated();
        }

        public DbSet<News> NewsItems => Set<News>();
        public DbSet<NewsImage> NewsImages => Set<NewsImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<News>()
                .Property(n => n.Category)
                .HasConversion<string>();
        }
    }
}
