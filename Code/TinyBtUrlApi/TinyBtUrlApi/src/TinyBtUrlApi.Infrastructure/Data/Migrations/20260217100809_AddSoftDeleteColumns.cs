using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TinyBtUrlApi.Infrastructure.Data.Migrations;

internal class _20260217100809_AddSoftDeleteColumns
{
  /// <inheritdoc />
  public partial class AddSoftDeleteColumns : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<DateTime>(
          name: "DeletedAt",
          table: "UrlMappings",
          type: "datetime2",
          nullable: true);

      migrationBuilder.AddColumn<bool>(
          name: "IsDeleted",
          table: "UrlMappings",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "DeletedAt",
          table: "UrlMappings");

      migrationBuilder.DropColumn(
          name: "IsDeleted",
          table: "UrlMappings");
    }
  }
}
