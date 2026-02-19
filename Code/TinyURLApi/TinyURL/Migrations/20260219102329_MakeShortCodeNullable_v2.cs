using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyURL.Migrations
{
    /// <inheritdoc />
    public partial class MakeShortCodeNullable_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlMappings_ShortCode",
                table: "UrlMappings");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "UrlMappings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "UrlMappings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UrlMappings_ShortCode",
                table: "UrlMappings",
                column: "ShortCode",
                unique: true,
                filter: "[ShortCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlMappings_ShortCode",
                table: "UrlMappings");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "UrlMappings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "UrlMappings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrlMappings_ShortCode",
                table: "UrlMappings",
                column: "ShortCode",
                unique: true);
        }
    }
}
