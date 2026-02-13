using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyURL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UrlMappings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrlMappings_UserId",
                table: "UrlMappings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UrlMappings_Users_UserId",
                table: "UrlMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrlMappings_Users_UserId",
                table: "UrlMappings");

            migrationBuilder.DropIndex(
                name: "IX_UrlMappings_UserId",
                table: "UrlMappings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UrlMappings");
        }
    }
}
