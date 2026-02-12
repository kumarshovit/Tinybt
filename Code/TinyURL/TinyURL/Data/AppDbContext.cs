using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlMapping>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();
        }
    }
}