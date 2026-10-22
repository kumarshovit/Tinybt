using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlSafetyFlagging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FlaggedAt",
                table: "UrlMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlaggedReason",
                table: "UrlMappings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "UrlMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSafetyCheckAt",
                table: "UrlMappings",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlaggedAt",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "FlaggedReason",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "LastSafetyCheckAt",
                table: "UrlMappings");
        }
    }
}
