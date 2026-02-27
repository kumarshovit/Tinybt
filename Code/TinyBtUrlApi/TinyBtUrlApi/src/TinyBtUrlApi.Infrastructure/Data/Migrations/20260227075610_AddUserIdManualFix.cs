using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyBtUrlApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdManualFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
      migrationBuilder.Sql(
 @"IF COL_LENGTH('UrlMappings', 'UserId') IS NULL
          ALTER TABLE UrlMappings ADD UserId INT NULL");
    }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
      migrationBuilder.Sql(
  @"IF COL_LENGTH('UrlMappings', 'UserId') IS NOT NULL
          ALTER TABLE UrlMappings DROP COLUMN UserId");
    }
    }
}
