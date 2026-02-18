using System.Xml.Serialization;
using Azure;
using Org.BouncyCastle.Asn1.Pkcs;
using TinyBtUrlApi.Core.ContributorAggregate;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
  public DbSet<Contributor> Contributors => Set<Contributor>();

  // 🔥 ADD THESE
  public DbSet<UrlMapping> UrlMappings => Set<UrlMapping>();
  public DbSet<Tag> Tags => Set<Tag>();
  public DbSet<UrlTag> UrlTags => Set<UrlTag>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        Assembly.GetExecutingAssembly());

    // 🔥 Move your Fluent API config here
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

  public override int SaveChanges() =>
      SaveChangesAsync().GetAwaiter().GetResult();
}
