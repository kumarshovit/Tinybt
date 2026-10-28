using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMicrosoftAuthToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MicrosoftId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMicrosoftAccount",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MicrosoftId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMicrosoftAccount",
                table: "Users");
        }
    }
}

