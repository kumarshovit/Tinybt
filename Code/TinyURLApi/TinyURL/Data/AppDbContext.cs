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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UrlTag> UrlTags { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlMapping>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            modelBuilder.Entity<UrlTag>()
    .HasKey(ut => new { ut.UrlMappingId, ut.TagId });

            modelBuilder.Entity<UrlTag>()
                .HasOne(ut => ut.UrlMapping)
                .WithMany(u => u.UrlTags)
                .HasForeignKey(ut => ut.UrlMappingId);

            modelBuilder.Entity<UrlTag>()
                .HasOne(ut => ut.Tag)
                .WithMany(t => t.UrlTags)
                .HasForeignKey(ut => ut.TagId);
        }
    }
}