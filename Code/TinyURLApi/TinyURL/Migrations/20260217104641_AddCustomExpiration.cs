using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyURL.Migrations
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
