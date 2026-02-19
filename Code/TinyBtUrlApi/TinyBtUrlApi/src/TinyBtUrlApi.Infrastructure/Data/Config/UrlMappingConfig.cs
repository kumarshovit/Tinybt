using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Infrastructure.Data.Config;

public class UrlMappingConfig : IEntityTypeConfiguration<UrlMapping>
{
  public void Configure(EntityTypeBuilder<UrlMapping> builder)
  {
    builder.HasIndex(x => x.ShortCode).IsUnique();
  }
}
