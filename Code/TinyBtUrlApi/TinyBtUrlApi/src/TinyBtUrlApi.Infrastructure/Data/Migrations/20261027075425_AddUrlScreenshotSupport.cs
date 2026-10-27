using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlScreenshotSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloudflareScanUuid",
                table: "UrlMappings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongUrlHash",
                table: "UrlMappings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScreenshotFetchedAt",
                table: "UrlMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotPath",
                table: "UrlMappings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScreenshotStatus",
                table: "UrlMappings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UrlMappings_LongUrlHash_ScreenshotStatus",
                table: "UrlMappings",
                columns: new[] { "LongUrlHash", "ScreenshotStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlMappings_LongUrlHash_ScreenshotStatus",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "CloudflareScanUuid",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "LongUrlHash",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "ScreenshotFetchedAt",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "ScreenshotPath",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "ScreenshotStatus",
                table: "UrlMappings");
        }
    }
}
