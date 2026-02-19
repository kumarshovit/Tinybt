using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyURL.Migrations
{
    /// <inheritdoc />
    public partial class FinalAnalyticsSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VisitorId",
                table: "ClickLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ClickLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RawHeaders",
                table: "ClickLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClickLogs_ClickedAt",
                table: "ClickLogs",
                column: "ClickedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ClickLogs_ShortCode",
                table: "ClickLogs",
                column: "ShortCode");

            migrationBuilder.CreateIndex(
                name: "IX_ClickLogs_VisitorId",
                table: "ClickLogs",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClickLogs_ClickedAt",
                table: "ClickLogs");

            migrationBuilder.DropIndex(
                name: "IX_ClickLogs_ShortCode",
                table: "ClickLogs");

            migrationBuilder.DropIndex(
                name: "IX_ClickLogs_VisitorId",
                table: "ClickLogs");

            migrationBuilder.AlterColumn<string>(
                name: "VisitorId",
                table: "ClickLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ClickLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RawHeaders",
                table: "ClickLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
