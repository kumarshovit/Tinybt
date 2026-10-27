using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Infrastructure.Data.Config;

public class UrlMappingConfig : IEntityTypeConfiguration<UrlMapping>
{
  public void Configure(EntityTypeBuilder<UrlMapping> builder)
  {
    builder.HasIndex(x => x.ShortCode).IsUnique();

    builder.Property(x => x.LongUrlHash).HasMaxLength(64);
    builder.Property(x => x.CloudflareScanUuid).HasMaxLength(100);
    builder.Property(x => x.ScreenshotPath).HasMaxLength(500);
    builder.Property(x => x.ScreenshotStatus)
      .HasDefaultValue(TinyBtUrlApi.Core.Models.ScreenshotStatus.None);

    builder.HasIndex(x => new { x.LongUrlHash, x.ScreenshotStatus });
  }
}
