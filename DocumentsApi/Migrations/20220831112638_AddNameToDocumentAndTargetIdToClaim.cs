using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentsApi.Migrations
{
    public partial class AddNameToDocumentAndTargetIdToClaim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "documents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "target_id",
                table: "claims",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "target_id",
                table: "claims");
        }
    }
}
