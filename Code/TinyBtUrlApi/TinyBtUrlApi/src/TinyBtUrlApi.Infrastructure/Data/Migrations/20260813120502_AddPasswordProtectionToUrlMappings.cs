using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordProtectionToUrlMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordProtected",
                table: "UrlMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "UrlMappings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPasswordProtected",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UrlMappings");
        }
    }
}
