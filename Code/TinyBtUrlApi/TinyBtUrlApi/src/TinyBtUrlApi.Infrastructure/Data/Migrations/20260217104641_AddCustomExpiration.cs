using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TinyBtUrlApi.Infrastructure.Data.Migrations;

internal class _20260217104641_AddCustomExpiration
{
  /// <inheritdoc />
  public partial class AddCustomExpiration : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<DateTime>(
          name: "ExpirationDate",
          table: "UrlMappings",
          type: "datetime2",
          nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "ExpirationDate",
          table: "UrlMappings");
    }
  }
}
