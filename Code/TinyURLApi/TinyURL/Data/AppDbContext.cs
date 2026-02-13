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

        // 👇 ADD THIS LINE
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlMapping>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            // 👇 Optional but Recommended (Make Email Unique)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
