using Microsoft.EntityFrameworkCore;
using TinyURL.Models;

namespace TinyURL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UrlMapping> UrlMappings { get; set; }
        public DbSet<ClickLog> ClickLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlMapping>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            modelBuilder.Entity<ClickLog>()
                .HasIndex(x => x.ShortCode);

            modelBuilder.Entity<ClickLog>()
                .HasIndex(x => x.ClickedAt);

            modelBuilder.Entity<ClickLog>()
                .HasIndex(x => x.VisitorId);
        }
    }
}
