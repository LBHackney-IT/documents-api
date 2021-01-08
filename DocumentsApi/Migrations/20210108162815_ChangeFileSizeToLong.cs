using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentsApi.Migrations
{
    public partial class ChangeFileSizeToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "file_size",
                table: "documents",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "file_size",
                table: "documents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
