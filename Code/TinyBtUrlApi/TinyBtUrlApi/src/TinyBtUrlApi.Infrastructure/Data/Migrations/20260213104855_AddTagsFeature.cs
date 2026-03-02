using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TinyBtUrlApi.Infrastructure.Data.Migrations;

internal class _20260213104855_AddTagsFeature
{
  /// <inheritdoc />
  public partial class AddTagsFeature : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Tags",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Tags", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "UrlTags",
          columns: table => new
          {
            UrlMappingId = table.Column<int>(type: "int", nullable: false),
            TagId = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UrlTags", x => new { x.UrlMappingId, x.TagId });
            table.ForeignKey(
                      name: "FK_UrlTags_Tags_TagId",
                      column: x => x.TagId,
                      principalTable: "Tags",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_UrlTags_UrlMappings_UrlMappingId",
                      column: x => x.UrlMappingId,
                      principalTable: "UrlMappings",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_UrlTags_TagId",
          table: "UrlTags",
          column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "UrlTags");

      migrationBuilder.DropTable(
          name: "Tags");
    }
  }
}
