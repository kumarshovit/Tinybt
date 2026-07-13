using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressToUrlMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "UrlMappings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "UrlMappings");
        }
    }
}
