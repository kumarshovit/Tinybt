using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  public DbSet<UrlMapping> UrlMappings { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<UrlTag> UrlTags { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // ⭐ Auto load all IEntityTypeConfiguration
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
      SaveChangesAsync().GetAwaiter().GetResult();
}
