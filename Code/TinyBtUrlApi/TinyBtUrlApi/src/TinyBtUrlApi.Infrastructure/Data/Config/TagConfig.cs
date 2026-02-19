using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Infrastructure.Data.Config;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
  public void Configure(EntityTypeBuilder<Tag> builder)
  {
    builder.Property(x => x.Name).IsRequired();
  }
}
