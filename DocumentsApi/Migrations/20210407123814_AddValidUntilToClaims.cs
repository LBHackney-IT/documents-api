using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentsApi.Migrations
{
    public partial class AddValidUntilToClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "valid_until",
                table: "claims",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "valid_until",
                table: "claims");
        }
    }
}
