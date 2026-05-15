using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TinyBtUrlApi.Infrastructure.Data;
using TinyBtUrlApi.Core.Models;

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
  public DbSet<User> Users => Set<User>();
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<RevokedToken> RevokedTokens { get; set; }
  public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
  public DbSet<IpLoginAttempt> IpLoginAttempts { get; set; }

  public DbSet<SystemSettings> SystemSettings { get; set; }

  public DbSet<ClickLog> ClickLogs { get; set; }
  public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // ⭐ Auto load all IEntityTypeConfiguration
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
      SaveChangesAsync().GetAwaiter().GetResult();
}
