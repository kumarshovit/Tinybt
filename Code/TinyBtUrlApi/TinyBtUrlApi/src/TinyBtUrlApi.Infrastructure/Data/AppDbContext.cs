using TinyBtUrlApi.Core.ContributorAggregate;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<User> Users => Set<User>();
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<RevokedToken> RevokedTokens { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
