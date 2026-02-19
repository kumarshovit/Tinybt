using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Infrastructure.Data.Config;

public class UrlTagConfig : IEntityTypeConfiguration<UrlTag>
{
  public void Configure(EntityTypeBuilder<UrlTag> builder)
  {
    builder.HasKey(x => new { x.UrlMappingId, x.TagId });

    builder.HasOne(x => x.UrlMapping)
        .WithMany(x => x.UrlTags)
        .HasForeignKey(x => x.UrlMappingId);

    builder.HasOne(x => x.Tag)
        .WithMany(x => x.UrlTags)
        .HasForeignKey(x => x.TagId);
  }
}
