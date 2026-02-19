using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyURL.Migrations
{
    /// <inheritdoc />
    public partial class AddRawFieldsToRequestLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawBody",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RouteValues",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "RawBody",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "RouteValues",
                table: "RequestLogs");
        }
    }
}
